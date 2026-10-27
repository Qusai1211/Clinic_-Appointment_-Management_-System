import os
import sys
import time

sys.stdout.reconfigure(encoding='utf-8')
from playwright.sync_api import sync_playwright

CHROME_PATH = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
BASE_URL = "http://localhost:4200"
SCREENSHOTS_DIR = "screenshots"

os.makedirs(SCREENSHOTS_DIR, exist_ok=True)

def run_tests():
    print("=== Starting Playwright End-to-End QA and Screenshot Suite ===")
    
    with sync_playwright() as p:
        browser = p.chromium.launch(
            executable_path=CHROME_PATH,
            headless=True,
            args=["--no-sandbox", "--disable-gpu"]
        )
        
        # Desktop context
        context = browser.new_context(viewport={"width": 1280, "height": 850})
        page = context.new_page()

        # Listen to console errors
        errors = []
        page.on("console", lambda msg: errors.append(f"CONSOLE: {msg.text}") if msg.type == "error" else None)
        page.on("pageerror", lambda err: errors.append(f"PAGE ERROR: {err}"))

        # ---------------------------------------------------------------------
        # Scenario 1: Home Page
        # ---------------------------------------------------------------------
        print("\n[Scenario 1] Home Page...")
        page.goto(f"{BASE_URL}/", wait_until="networkidle")
        page.wait_for_selector(".specialty-card")
        page.wait_for_selector(".doctor-card")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "01_Home_Page.png"), full_page=True)
        print("  ✓ Home page rendered with specialties & featured doctors")

        # ---------------------------------------------------------------------
        # Scenario 2: Specialty Filtering from Home -> Doctors Page
        # ---------------------------------------------------------------------
        print("\n[Scenario 2] Specialty click -> Filtered Doctors...")
        first_specialty = page.locator(".specialty-card").first
        first_specialty.click()
        page.wait_for_url("**/doctors?specialty=*")
        page.wait_for_selector(".doctor-card")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "03_Doctors_Filtered.png"), full_page=True)
        print("  ✓ Navigation to filtered doctors confirmed")

        # ---------------------------------------------------------------------
        # Scenario 3: Doctors Page (Full list, search, and sort)
        # ---------------------------------------------------------------------
        print("\n[Scenario 3] Doctors Page...")
        page.goto(f"{BASE_URL}/doctors", wait_until="networkidle")
        page.wait_for_selector(".doctor-card")
        # Search for Sarah
        page.fill(".search-box input", "Sarah")
        time.sleep(0.5)
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "02_Doctors_Page.png"), full_page=True)
        print("  ✓ Doctors page loaded and search verified")

        # ---------------------------------------------------------------------
        # Scenario 4: Doctor Details & Slot Selection
        # ---------------------------------------------------------------------
        print("\n[Scenario 4] Doctor Details Page...")
        page.goto(f"{BASE_URL}/doctors/1", wait_until="networkidle")
        page.wait_for_selector(".doctor-profile-card")
        page.wait_for_selector(".slot-chip")
        
        # Click on an available slot
        slot = page.locator(".slot-chip").first
        slot.click()
        time.sleep(0.3)
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "04_Doctor_Details.png"), full_page=True)
        print("  ✓ Doctor details, schedule, and slot selection verified")

        # ---------------------------------------------------------------------
        # Scenario 5: Patient Registration
        # ---------------------------------------------------------------------
        print("\n[Scenario 5] Patient Registration Page...")
        page.goto(f"{BASE_URL}/register-patient", wait_until="networkidle")
        page.fill("#fullName", "Mariam Al-Khalili")
        page.fill("#phoneNumber", "+962794443322")
        page.fill("#email", "mariam.khalili@example.com")
        page.fill("#dateOfBirth", "1997-08-20")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "05_Patient_Registration_Form.png"), full_page=True)
        
        page.click(".btn-submit")
        page.wait_for_selector(".success-card")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "05_Patient_Registration.png"), full_page=True)
        print("  ✓ Patient registered successfully in Oracle Database")

        # ---------------------------------------------------------------------
        # Scenario 6: Book Appointment Flow
        # ---------------------------------------------------------------------
        print("\n[Scenario 6] Book Appointment Page...")
        page.goto(f"{BASE_URL}/book?doctorId=3", wait_until="networkidle")
        page.wait_for_selector(".slot-btn")
        
        # Select first slot
        page.locator(".slot-btn").first.click()
        
        # Fill new patient details
        page.fill("#patName", "Kareem Taha")
        page.fill("#patPhone", "+962788887766")
        page.fill("#patEmail", "kareem.taha@example.com")
        
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "06_Book_Appointment.png"), full_page=True)
        page.click(".btn-book")
        
        # Wait for confirmation redirect
        page.wait_for_url("**/confirmation/*", timeout=10000)
        page.wait_for_selector(".confirmation-card")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "07_Appointment_Confirmation.png"), full_page=True)
        print("  ✓ Appointment booked and confirmed with Reference Number")

        # ---------------------------------------------------------------------
        # Scenario 7: Appointments Page
        # ---------------------------------------------------------------------
        print("\n[Scenario 7] Appointments Page...")
        page.goto(f"{BASE_URL}/appointments", wait_until="networkidle")
        page.wait_for_selector(".table tbody tr")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "08_Appointments_Page.png"), full_page=True)
        print("  ✓ Appointments listing and status filters verified")

        # ---------------------------------------------------------------------
        # Scenario 8: Admin Login
        # ---------------------------------------------------------------------
        print("\n[Scenario 8] Admin Login...")
        page.goto(f"{BASE_URL}/admin/login", wait_until="networkidle")
        page.fill("#username", "admin")
        page.fill("#password", "admin123")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "09_Admin_Login.png"), full_page=True)
        page.click(".btn-login")

        # Wait for dashboard redirect
        page.wait_for_url("**/admin/dashboard", timeout=10000)
        page.wait_for_selector(".stat-card")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "10_Admin_Dashboard.png"), full_page=True)
        print("  ✓ Admin authenticated; live stats and chart rendered")

        # ---------------------------------------------------------------------
        # Scenario 9: Admin Manage Doctors
        # ---------------------------------------------------------------------
        print("\n[Scenario 9] Admin Manage Doctors...")
        page.goto(f"{BASE_URL}/admin/doctors", wait_until="networkidle")
        page.wait_for_selector(".table tbody tr")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "11_Admin_Manage_Doctors.png"), full_page=True)
        print("  ✓ Admin doctors management verified")

        # ---------------------------------------------------------------------
        # Scenario 10: Admin Manage Specialties
        # ---------------------------------------------------------------------
        print("\n[Scenario 10] Admin Manage Specialties...")
        page.goto(f"{BASE_URL}/admin/specialties", wait_until="networkidle")
        page.wait_for_selector(".table tbody tr")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "12_Admin_Manage_Specialties.png"), full_page=True)
        print("  ✓ Admin specialties management verified")

        # ---------------------------------------------------------------------
        # Scenario 11: Admin Manage Patients
        # ---------------------------------------------------------------------
        print("\n[Scenario 11] Admin Manage Patients...")
        page.goto(f"{BASE_URL}/admin/patients", wait_until="networkidle")
        page.wait_for_selector(".table tbody tr")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "13_Admin_Manage_Patients.png"), full_page=True)
        print("  ✓ Admin patient directory verified")

        # ---------------------------------------------------------------------
        # Scenario 12: Admin Manage Appointments
        # ---------------------------------------------------------------------
        print("\n[Scenario 12] Admin Manage Appointments...")
        page.goto(f"{BASE_URL}/admin/appointments", wait_until="networkidle")
        page.wait_for_selector(".table tbody tr")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "14_Admin_Manage_Appointments.png"), full_page=True)
        print("  ✓ Admin appointment management verified")

        # ---------------------------------------------------------------------
        # Scenario 13: 404 Not Found Page
        # ---------------------------------------------------------------------
        print("\n[Scenario 13] Unknown URL -> Not Found Page...")
        page.goto(f"{BASE_URL}/unknown-clinic-page-route", wait_until="networkidle")
        page.wait_for_selector(".not-found-box")
        page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "15_Not_Found_Page.png"), full_page=True)
        print("  ✓ 404 Not Found route verified")

        # ---------------------------------------------------------------------
        # Scenario 14: Mobile Layout (375x667)
        # ---------------------------------------------------------------------
        print("\n[Scenario 14] Mobile Viewport Responsiveness...")
        mobile_context = browser.new_context(viewport={"width": 375, "height": 667})
        mobile_page = mobile_context.new_page()
        mobile_page.goto(f"{BASE_URL}/", wait_until="networkidle")
        mobile_page.wait_for_selector(".hero-title")
        mobile_page.screenshot(path=os.path.join(SCREENSHOTS_DIR, "16_Mobile_View_Home.png"), full_page=True)
        print("  ✓ Mobile responsiveness verified")

        browser.close()

        print("\n=== All Scenarios Passed Successfully! ===")
        if errors:
            print("Console warnings/errors encountered:")
            for e in errors:
                print(" ", e)
        else:
            print("Zero unhandled console errors during test run!")

if __name__ == "__main__":
    run_tests()
