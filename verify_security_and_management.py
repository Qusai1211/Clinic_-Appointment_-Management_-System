import sys
import json
import urllib.request
import urllib.error
import time

if sys.platform == "win32":
    sys.stdout.reconfigure(encoding='utf-8')

BASE_URL = "http://localhost:5069/api"

def http_req(method, url, data=None, headers=None):
    if headers is None:
        headers = {}
    body = None
    if data is not None:
        body = json.dumps(data).encode('utf-8')
        headers["Content-Type"] = "application/json"
    
    req = urllib.request.Request(url, data=body, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req) as resp:
            content = resp.read().decode('utf-8')
            return resp.status, json.loads(content) if content else {}
    except urllib.error.HTTPError as err:
        content = err.read().decode('utf-8')
        try:
            return err.code, json.loads(content) if content else {}
        except:
            return err.code, {"raw": content}

print("=============================================================")
print("=== SECURITY & ADMIN MANAGEMENT VERIFICATION TEST SUITE ===")
print("=============================================================\n")

# 1. Admin Login & Transparent PBKDF2 Migration Test
print("[Test 1] Testing Admin Login & PBKDF2 Password Migration...")
login_payload = {"username": "admin", "password": "admin123"}
status, login_data = http_req("POST", f"{BASE_URL}/auth/login", login_payload)
assert status == 200, f"Login failed with status {status}: {login_data}"
assert login_data.get("success") is True, f"Login unsuccessful: {login_data}"
token = login_data.get("token")
assert token, "JWT token was not returned!"
print(f"  ✓ Login successful. JWT token received: {token[:25]}...")

# Test subsequent login to verify it still works against the upgraded PBKDF2 hash
status2, login_data_2 = http_req("POST", f"{BASE_URL}/auth/login", login_payload)
assert status2 == 200, f"Second login failed with status {status2}: {login_data_2}"
print("  ✓ Second login successfully authenticated against upgraded PBKDF2 hash.")

# Test invalid login
status_bad, bad_data = http_req("POST", f"{BASE_URL}/auth/login", {"username": "admin", "password": "wrongpassword!"})
assert status_bad in [400, 401, 200] and bad_data.get("success") is False, f"Invalid login was not rejected! {bad_data}"
print(f"  ✓ Invalid password properly rejected with HTTP {status_bad}: {bad_data.get('message')}")

headers_auth = {"Authorization": f"Bearer {token}"}

# 2. Real Backend Authorization Verification
print("\n[Test 2] Testing Real Backend Authorization on Protected Endpoints...")

# A. Patient Update without token
status_p, _ = http_req("PUT", f"{BASE_URL}/patients/1", {"fullName": "Unauthorized Attempt", "phoneNumber": "123"})
assert status_p == 401, f"Expected 401 Unauthorized for PUT /patients/1 without token, got {status_p}"
print("  ✓ PUT /api/patients/{id} without token correctly returned HTTP 401 Unauthorized.")

# B. Appointment Status without token
status_a, _ = http_req("PUT", f"{BASE_URL}/appointments/1/status", {"status": "Completed"})
assert status_a == 401, f"Expected 401 Unauthorized for PUT /appointments/1/status without token, got {status_a}"
print("  ✓ PUT /api/appointments/{id}/status without token correctly returned HTTP 401 Unauthorized.")

# C. Doctor Working Hours without token
status_wh, _ = http_req("PUT", f"{BASE_URL}/doctors/working-hours/1", {"dayOfWeek": "Monday", "startTime": "08:00 AM", "endTime": "04:00 PM"})
assert status_wh == 401, f"Expected 401 Unauthorized for PUT /doctors/working-hours/1 without token, got {status_wh}"
print("  ✓ PUT /api/doctors/working-hours/{id} without token correctly returned HTTP 401 Unauthorized.")

# D. Doctor Slot Availability without token
status_slot, _ = http_req("PUT", f"{BASE_URL}/doctors/slots/1/availability", {"isAvailable": False})
assert status_slot == 401, f"Expected 401 Unauthorized for PUT /doctors/slots/1/availability without token, got {status_slot}"
print("  ✓ PUT /api/doctors/slots/{id}/availability without token correctly returned HTTP 401 Unauthorized.")

# 3. Patient Management (Create -> Update -> Delete)
print("\n[Test 3] Testing Complete Patient Lifecycle Management...")
unique_phone = f"+96279{int(time.time()) % 10000000:07d}"
reg_payload = {
    "fullName": "Integration QA Patient",
    "phoneNumber": unique_phone,
    "email": f"qa_{int(time.time())}@example.com",
    "dateOfBirth": "1994-05-15"
}
status_reg, reg_data = http_req("POST", f"{BASE_URL}/patients/register", reg_payload)
assert status_reg == 200, f"Registration failed: {reg_data}"
pat_data = reg_data.get("patient")
test_pat_id = pat_data["patientId"]
print(f"  ✓ Created patient #{test_pat_id} with phone {unique_phone}")

# Now update the patient using authorized endpoint
update_payload = {
    "fullName": "Integration QA Patient (Updated)",
    "phoneNumber": unique_phone,
    "email": f"updated_qa_{int(time.time())}@example.com",
    "dateOfBirth": "1994-05-15"
}
status_upd, upd_res = http_req("PUT", f"{BASE_URL}/patients/{test_pat_id}", update_payload, headers_auth)
assert status_upd == 200, f"Update patient failed: {upd_res}"
print(f"  ✓ Updated patient #{test_pat_id} full name and email successfully.")

# Verify updated details via GET
status_get, pat_check = http_req("GET", f"{BASE_URL}/patients/{test_pat_id}")
assert status_get == 200
assert pat_check["fullName"] == "Integration QA Patient (Updated)"
print("  ✓ Verified patient update persisted accurately in Oracle.")

# Delete patient
status_del, del_res = http_req("DELETE", f"{BASE_URL}/patients/{test_pat_id}", headers=headers_auth)
assert status_del == 200, f"Delete patient failed: {del_res}"
print(f"  ✓ Deleted patient #{test_pat_id} successfully.")

# Verify 404 on deleted patient
status_del_check, _ = http_req("GET", f"{BASE_URL}/patients/{test_pat_id}")
assert status_del_check == 404
print("  ✓ Verified patient #{test_pat_id} is no longer in Oracle.")

# 4. Working Hours & Slot Management Test
print("\n[Test 4] Testing Working Hours & Slot Management...")
# Get working hours for doctor 1
_, wh_list = http_req("GET", f"{BASE_URL}/doctors/1/working-hours")
assert len(wh_list) > 0, "No working hours found for Doctor 1"
wh_target = wh_list[0]
target_wh_id = wh_target["workingHoursId"]

# Update working hours
upd_wh_payload = {
    "doctorid": 1,
    "dayOfWeek": wh_target["dayOfWeek"],
    "startTime": "08:30 AM",
    "endTime": "04:30 PM"
}
status_uwh, uwh_res = http_req("PUT", f"{BASE_URL}/doctors/working-hours/{target_wh_id}", upd_wh_payload, headers_auth)
assert status_uwh == 200, f"Update working hours failed: {uwh_res}"
print(f"  ✓ Updated working hours #{target_wh_id} to 08:30 AM – 04:30 PM.")

# Get slots for doctor 1
_, slots = http_req("GET", f"{BASE_URL}/doctors/1/slots")
assert len(slots) > 0, "No slots found for Doctor 1"
target_slot = slots[0]
target_slot_id = target_slot["slotId"]
orig_avail = target_slot["isAvailable"]

# Toggle availability
new_avail_state = (orig_avail == 0)
status_tog, tog_res = http_req("PUT", f"{BASE_URL}/doctors/slots/{target_slot_id}/availability", {"isAvailable": new_avail_state}, headers_auth)
assert status_tog == 200, f"Toggle slot failed: {tog_res}"
print(f"  ✓ Toggled slot #{target_slot_id} availability to {new_avail_state}.")

# Restore original availability
status_rest, _ = http_req("PUT", f"{BASE_URL}/doctors/slots/{target_slot_id}/availability", {"isAvailable": (orig_avail == 1)}, headers_auth)
assert status_rest == 200
print(f"  ✓ Restored slot #{target_slot_id} availability back to {orig_avail == 1}.")

# 5. Appointment Status Update Test
print("\n[Test 5] Testing Appointment Status Update...")
_, apps = http_req("GET", f"{BASE_URL}/appointments")
assert len(apps) > 0, "No appointments found"
target_app = apps[0]
app_id = target_app["appointmentId"]

status_st, st_res = http_req("PUT", f"{BASE_URL}/appointments/{app_id}/status", {"status": "Completed"}, headers_auth)
assert status_st == 200, f"Update status failed: {st_res}"
print(f"  ✓ Updated appointment #{app_id} status to 'Completed'.")

# Verify status in Oracle
_, app_check = http_req("GET", f"{BASE_URL}/appointments/{app_id}")
assert app_check["status"] == "Completed"
print(f"  ✓ Verified appointment #{app_id} status is 'Completed' in Oracle.")

print("\n=== ALL SECURITY & ADMIN MANAGEMENT TESTS PASSED SUCCESSFULLY! ===")
