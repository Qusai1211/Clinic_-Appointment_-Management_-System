import os
import sys
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.enum.text import PP_ALIGN
from pptx.dml.color import RGBColor
from pptx.enum.shapes import MSO_SHAPE

os.makedirs("presentation", exist_ok=True)
prs = Presentation()
# Set 16:9 widescreen dimensions
prs.slide_width = Inches(13.333)
prs.slide_height = Inches(7.5)

# Color Palette
NAVY = RGBColor(26, 54, 93)      # #1A365D
TEAL = RGBColor(13, 148, 136)    # #0D9488
DARK_SLATE = RGBColor(30, 41, 59)# #1E293B
LIGHT_BG = RGBColor(248, 250, 252) # #F8FAFC
WHITE = RGBColor(255, 255, 255)
GRAY = RGBColor(100, 116, 139)   # #64748B
LIGHT_TEAL = RGBColor(240, 253, 250)

def add_header(slide, title_text, category_text="CLINIC APPOINTMENT MANAGEMENT SYSTEM"):
    # Header banner
    banner = slide.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(1.1))
    banner.fill.solid()
    banner.fill.fore_color.rgb = NAVY
    banner.line.color.rgb = NAVY

    # Category / Subtitle small
    cat_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.12), Inches(11.5), Inches(0.3))
    tf_cat = cat_box.text_frame
    tf_cat.word_wrap = True
    p_cat = tf_cat.paragraphs[0]
    p_cat.text = category_text.upper()
    p_cat.font.size = Pt(10)
    p_cat.font.bold = True
    p_cat.font.color.rgb = TEAL

    # Main Slide Title
    title_box = slide.shapes.add_textbox(Inches(0.8), Inches(0.38), Inches(11.5), Inches(0.6))
    tf = title_box.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = title_text
    p.font.size = Pt(22)
    p.font.bold = True
    p.font.color.rgb = WHITE

# ----------------- SLIDE 1: Title Slide -----------------
blank_layout = prs.slide_layouts[6]
s1 = prs.slides.add_slide(blank_layout)

# Full slide background
bg1 = s1.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0), Inches(0), Inches(13.333), Inches(7.5))
bg1.fill.solid()
bg1.fill.fore_color.rgb = NAVY
bg1.line.color.rgb = NAVY

# Title Card box
card1 = s1.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(1.5), Inches(1.2), Inches(10.333), Inches(5.1))
card1.fill.solid()
card1.fill.fore_color.rgb = WHITE
card1.line.color.rgb = TEAL

tbox1 = s1.shapes.add_textbox(Inches(2.0), Inches(1.5), Inches(9.333), Inches(4.5))
tf1 = tbox1.text_frame
tf1.word_wrap = True

p1 = tf1.paragraphs[0]
p1.text = "FINAL PROJECT #9"
p1.font.size = Pt(14)
p1.font.bold = True
p1.font.color.rgb = TEAL

p2 = tf1.add_paragraph()
p2.text = "Clinic Appointment Management System"
p2.font.size = Pt(32)
p2.font.bold = True
p2.font.color.rgb = NAVY

p3 = tf1.add_paragraph()
p3.text = "Al-Amal Medical Center — Enterprise Full-Stack Healthcare Platform"
p3.font.size = Pt(16)
p3.font.color.rgb = GRAY
p3.space_after = Pt(20)

p4 = tf1.add_paragraph()
p4.text = "• Technology Stack: Oracle Database 19c | ASP.NET Core 8 Web API | Angular 21\n" \
          "• Key Capabilities: Double-Booking Concurrency Protection, Row-Level Locks, Live Admin Analytics, JWT Auth\n" \
          "• Academic Evaluation: Final Project University Specification Integration & Verification"
p4.font.size = Pt(13)
p4.font.color.rgb = DARK_SLATE
p4.space_after = Pt(25)

p5 = tf1.add_paragraph()
p5.text = "Presented by Full-Stack Engineering Team | University Computer Science / Software Engineering"
p5.font.size = Pt(12)
p5.font.bold = True
p5.font.color.rgb = NAVY

# ----------------- SLIDE 2: Executive Summary -----------------
s2 = prs.slides.add_slide(blank_layout)
add_header(s2, "Project Overview & Executive Summary")

# Left box: Problem & Mission
box_left = s2.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(1.5), Inches(5.6), Inches(5.3))
box_left.fill.solid()
box_left.fill.fore_color.rgb = WHITE
box_left.line.color.rgb = GRAY

tf_left = box_left.text_frame
tf_left.word_wrap = True
p = tf_left.paragraphs[0]
p.text = "Mission & Core Objectives"
p.font.size = Pt(18)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(12)

bullets_left = [
    "Digital Transformation: Transition clinic appointment workflows from legacy manual recording to an automated digital portal.",
    "Patient Convenience: Patients can discover specialized doctors, review verified schedules, select exact available slots, and obtain immediate confirmation.",
    "Concurrency & Integrity: Guarantee zero double-bookings under concurrent client requests using Oracle PL/SQL row-level locking.",
    "Administrative Empowerment: Real-time clinical KPI dashboard, dynamic specialty management, doctor profiles, and appointment lifecycle tracking.",
    "Authoritative Compliance: Full conformity with University Assignment Specification #9 requirements and architectural standards."
]
for b in bullets_left:
    p = tf_left.add_paragraph()
    p.text = "• " + b
    p.font.size = Pt(12)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(8)

# Right box: Architectural Highlights
box_right = s2.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(6.9), Inches(1.5), Inches(5.6), Inches(5.3))
box_right.fill.solid()
box_right.fill.fore_color.rgb = LIGHT_TEAL
box_right.line.color.rgb = TEAL

tf_right = box_right.text_frame
tf_right.word_wrap = True
p = tf_right.paragraphs[0]
p.text = "Architectural Highlights"
p.font.size = Pt(18)
p.font.bold = True
p.font.color.rgb = TEAL
p.space_after = Pt(12)

bullets_right = [
    "Frontend (Angular 21): Modern Single Page Application with standalone components, Zone.js change detection, reactive forms, and responsive LESS styling.",
    "Backend (ASP.NET Core 8 Web API): Clean 3-tier modular architecture (API, Core, Infrastructure) utilizing Dapper ORM and Oracle.ManagedDataAccess.Core.",
    "Database (Oracle 19c Enterprise): Relational normalized schema, foreign key cascades, constraints, views, and stored procedures with atomic transactions.",
    "Security: PBKDF2 password hashing (100k iterations, 128-bit salt), transparent legacy migration, JWT Bearer tokens, and protected Angular route guards.",
    "Zero-Stub Integration: All UI views and analytics connect to real running Oracle database instances with live queries."
]
for b in bullets_right:
    p = tf_right.add_paragraph()
    p.text = "✔ " + b
    p.font.size = Pt(12)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(8)

# ----------------- SLIDE 3: System Architecture -----------------
s3 = prs.slides.add_slide(blank_layout)
add_header(s3, "High-Level 3-Tier System Architecture")

tiers = [
    ("Presentation Tier", "Angular 21 Single Page App", [
        "Modern Component Architecture",
        "Reactive Forms & Client Validation",
        "RxJS & HttpClient Services",
        "Role-Based Route Guards",
        "Toast Notification Feedback",
        "Responsive Mobile / Desktop UI"
    ], Inches(0.8), Inches(1.5), Inches(3.6)),
    ("Application Tier", "ASP.NET Core 8 Web API", [
        "Clean 3-Tier Architecture",
        "Controllers / Services / Repositories",
        "JWT Bearer Authentication",
        "Dapper High-Performance Micro-ORM",
        "CORS & Swagger OpenAPI Spec",
        "Centralized Exception Handling"
    ], Inches(4.866), Inches(1.5), Inches(3.6)),
    ("Database Tier", "Oracle Database 19c", [
        "7 Relational Tables & Constraints",
        "PL/SQL Package (PKG_CLINIC_OPERATIONS)",
        "Row-Level FOR UPDATE Concurrency Locks",
        "Live Aggregation Views",
        "ACID Transaction Compliance",
        "Oracle.ManagedDataAccess.Core Driver"
    ], Inches(8.933), Inches(1.5), Inches(3.6))
]

for title, sub, items, left, top, width in tiers:
    card = s3.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, Inches(5.3))
    card.fill.solid()
    card.fill.fore_color.rgb = WHITE
    card.line.color.rgb = TEAL

    tf = card.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = title
    p.font.size = Pt(16)
    p.font.bold = True
    p.font.color.rgb = NAVY

    p_sub = tf.add_paragraph()
    p_sub.text = sub
    p_sub.font.size = Pt(12)
    p_sub.font.bold = True
    p_sub.font.color.rgb = TEAL
    p_sub.space_after = Pt(14)

    for it in items:
        p_it = tf.add_paragraph()
        p_it.text = "• " + it
        p_it.font.size = Pt(11)
        p_it.font.color.rgb = DARK_SLATE
        p_it.space_after = Pt(6)

# ----------------- SLIDE 4: Database Design & ERD -----------------
s4 = prs.slides.add_slide(blank_layout)
add_header(s4, "Oracle Database Architecture & Relational Schema")

erd_path = "database_project/erdplus.png"
if os.path.exists(erd_path):
    s4.shapes.add_picture(erd_path, Inches(0.8), Inches(1.5), width=Inches(7.2))

# Right side: Schema description
box_erd = s4.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(8.3), Inches(1.5), Inches(4.2), Inches(5.3))
box_erd.fill.solid()
box_erd.fill.fore_color.rgb = WHITE
box_erd.line.color.rgb = GRAY

tf_erd = box_erd.text_frame
tf_erd.word_wrap = True
p = tf_erd.paragraphs[0]
p.text = "Database Structure"
p.font.size = Pt(16)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(10)

erd_notes = [
    "Specialties: Medical departments (Cardiology, Pediatrics, Dermatology, etc.).",
    "Doctors: Physician profiles, specialization foreign key, consultation fees, and experience.",
    "DoctorWorkingHours: Weekly operating schedules by day with start/end time windows.",
    "AppointmentSlots: Specific bookable time slots per doctor with date and availability flag.",
    "Patients: Registered patient registry with unique phone & email constraints.",
    "Appointments: Persisted bookings linked to patient, doctor, and slot with statuses.",
    "Admins: Secure administrative credentials storing salted SHA-256 hashes."
]
for n in erd_notes:
    p = tf_erd.add_paragraph()
    p.text = "• " + n
    p.font.size = Pt(10)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(6)

# ----------------- SLIDE 5: Advanced Oracle Features & Concurrency -----------------
s5 = prs.slides.add_slide(blank_layout)
add_header(s5, "PL/SQL Stored Procedures & Double-Booking Prevention")

# Left Column: PL/SQL Package
box_pkg = s5.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(1.5), Inches(5.6), Inches(5.3))
box_pkg.fill.solid()
box_pkg.fill.fore_color.rgb = WHITE
box_pkg.line.color.rgb = TEAL

tf_pkg = box_pkg.text_frame
tf_pkg.word_wrap = True
p = tf_pkg.paragraphs[0]
p.text = "Package PKG_CLINIC_OPERATIONS"
p.font.size = Pt(16)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(10)

pkg_items = [
    ("REGISTER_PATIENT", "Atomically registers or returns existing patient by phone number. Prevents duplicate identities."),
    ("BOOK_APPOINTMENT", "Executes pessimistic slot locking using SELECT ... FOR UPDATE. Validates slot doctor and date. Inserts appointment and flips slot availability to 0 atomically."),
    ("CANCEL_APPOINTMENT", "Updates appointment status to 'Cancelled' and automatically releases the associated slot back to Available (1)."),
    ("UPDATE_APPOINTMENT_STATUS", "Safely transitions appointment states (Scheduled -> Completed / Cancelled). Reopens slot if cancelled.")
]
for name, desc in pkg_items:
    p = tf_pkg.add_paragraph()
    p.text = "⚙ " + name
    p.font.size = Pt(12)
    p.font.bold = True
    p.font.color.rgb = TEAL
    p_desc = tf_pkg.add_paragraph()
    p_desc.text = desc
    p_desc.font.size = Pt(10)
    p_desc.font.color.rgb = DARK_SLATE
    p_desc.space_after = Pt(8)

# Right Column: Concurrency & Race Condition Handling
box_conc = s5.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(6.9), Inches(1.5), Inches(5.6), Inches(5.3))
box_conc.fill.solid()
box_conc.fill.fore_color.rgb = LIGHT_BG
box_conc.line.color.rgb = GRAY

tf_conc = box_conc.text_frame
tf_conc.word_wrap = True
p = tf_conc.paragraphs[0]
p.text = "Double-Booking Prevention Mechanism"
p.font.size = Pt(16)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(10)

conc_points = [
    "The Problem: When two patients simultaneously click 'Confirm Booking' for the exact same slot, naive systems create duplicate conflicting appointments.",
    "Oracle Row-Level Lock: The procedure executes:\nSELECT IsAvailable INTO v_IsAvailable FROM AppointmentSlots WHERE SlotId = p_SlotId FOR UPDATE;\nThis immediately serializes competing transactions at the database engine level.",
    "Immediate Rejection: If v_IsAvailable = 0, the procedure raises ORA-20002 ('Selected appointment slot is no longer available').",
    "API Conflict Mapping: The ASP.NET Core service catches the Oracle exception and translates it into HTTP 409 Conflict with a clear message.",
    "Automated Test Verified: Concurrency test verified that parallel requests for the same slot result in exactly 1 successful booking and 1 HTTP 409 rejection."
]
for cp in conc_points:
    p = tf_conc.add_paragraph()
    p.text = "🔒 " + cp
    p.font.size = Pt(11)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(6)

# ----------------- SLIDE 6: ASP.NET Core Backend Implementation -----------------
s6 = prs.slides.add_slide(blank_layout)
add_header(s6, "ASP.NET Core Web API Backend Architecture")

cards_backend = [
    ("Core Layer", "Domain Models & Interfaces", [
        "Pure C# domain models (Doctor, Patient, Appointment, Slot)",
        "Strongly typed DTOs for requests and responses",
        "Repository & Service interface abstractions",
        "Zero external dependencies for maximum testability"
    ], Inches(0.8), Inches(1.5), Inches(3.6)),
    ("Infrastructure Layer", "Oracle Data Access & Services", [
        "Dapper high-speed micro-ORM queries",
        "Oracle.ManagedDataAccess.Core integration",
        "PL/SQL package stored procedure execution",
        "DynamicParameters for input/output cursors",
        "PBKDF2 PasswordSecurity (100k iterations, random 128-bit salt)",
        "JWT token generation with symmetric security keys"
    ], Inches(4.866), Inches(1.5), Inches(3.6)),
    ("API Layer", "Controllers & Middleware", [
        "RESTful endpoints adhering to HTTP standards",
        "JWT Bearer Claims-based Authorization",
        "CORS policy enabled for Angular frontend",
        "Swagger / OpenAPI interactive UI",
        "HTTP Status Codes: 200, 201, 400, 404, 409 Conflict"
    ], Inches(8.933), Inches(1.5), Inches(3.6))
]

for title, sub, items, left, top, width in cards_backend:
    card = s6.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, Inches(5.3))
    card.fill.solid()
    card.fill.fore_color.rgb = WHITE
    card.line.color.rgb = TEAL

    tf = card.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = title
    p.font.size = Pt(16)
    p.font.bold = True
    p.font.color.rgb = NAVY

    p_sub = tf.add_paragraph()
    p_sub.text = sub
    p_sub.font.size = Pt(11)
    p_sub.font.bold = True
    p_sub.font.color.rgb = TEAL
    p_sub.space_after = Pt(12)

    for it in items:
        p_it = tf.add_paragraph()
        p_it.text = "• " + it
        p_it.font.size = Pt(11)
        p_it.font.color.rgb = DARK_SLATE
        p_it.space_after = Pt(6)

# ----------------- SLIDE 7: Angular Frontend Experience -----------------
s7 = prs.slides.add_slide(blank_layout)
add_header(s7, "Angular 21 Frontend Architecture & User Experience")

home_pic = "screenshots/01_Home_Page.png"
if os.path.exists(home_pic):
    s7.shapes.add_picture(home_pic, Inches(0.8), Inches(1.5), width=Inches(6.8))

box_fe = s7.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(7.9), Inches(1.5), Inches(4.6), Inches(5.3))
box_fe.fill.solid()
box_fe.fill.fore_color.rgb = WHITE
box_fe.line.color.rgb = GRAY

tf_fe = box_fe.text_frame
tf_fe.word_wrap = True
p = tf_fe.paragraphs[0]
p.text = "Key Frontend Highlights"
p.font.size = Pt(16)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(10)

fe_points = [
    "Standalone Architecture: Built on Angular 21 with modern standalone components and zero module boilerplate.",
    "Zone.js Integration: Configured provideZoneChangeDetection with markForCheck() for deterministic reactivity across all asynchronous RxJS calls.",
    "Form Validation: Angular ReactiveForms with real-time feedback for required fields, phone numbers, and email format.",
    "Unified Navigation: Global Navbar with responsive mobile toggle, breadcrumbs, and deep-link query parameter support.",
    "Notification Toasts: Non-intrusive floating alerts for successful actions and informative error diagnostics.",
    "Strict Route Guarding: Admin portal secured via CanActivate AuthGuard redirecting unauthenticated traffic to /admin/login."
]
for fp in fe_points:
    p = tf_fe.add_paragraph()
    p.text = "✔ " + fp
    p.font.size = Pt(11)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(7)

# ----------------- SLIDE 8: Public Clinical Booking Workflow -----------------
s8 = prs.slides.add_slide(blank_layout)
add_header(s8, "End-to-End Patient Booking Workflow")

doc_pic = "screenshots/04_Doctor_Details.png"
conf_pic = "screenshots/07_Appointment_Confirmation.png"

if os.path.exists(doc_pic):
    s8.shapes.add_picture(doc_pic, Inches(0.8), Inches(1.5), width=Inches(5.7))

if os.path.exists(conf_pic):
    s8.shapes.add_picture(conf_pic, Inches(6.8), Inches(1.5), width=Inches(5.7))

# Bottom banner describing steps
steps_bar = s8.shapes.add_shape(MSO_SHAPE.RECTANGLE, Inches(0.8), Inches(5.7), Inches(11.7), Inches(1.3))
steps_bar.fill.solid()
steps_bar.fill.fore_color.rgb = NAVY
steps_bar.line.color.rgb = TEAL

tf_sb = steps_bar.text_frame
tf_sb.word_wrap = True
p = tf_sb.paragraphs[0]
p.text = "The 5-Step Seamless Patient Booking Journey"
p.font.size = Pt(13)
p.font.bold = True
p.font.color.rgb = WHITE

p_steps = tf_sb.add_paragraph()
p_steps.text = "Step 1: Explore Specialties & Doctors  ➔  Step 2: Review Schedule & Select Available Slot  ➔  Step 3: Enter Patient Details  ➔  Step 4: Atomic DB Reservation  ➔  Step 5: Instant Confirmation with Unique Reference Number"
p_steps.font.size = Pt(10)
p_steps.font.color.rgb = TEAL

# ----------------- SLIDE 9: Administrative Portal & Analytics -----------------
s9 = prs.slides.add_slide(blank_layout)
add_header(s9, "Admin Dashboard & Operational Management")

dash_pic = "screenshots/10_Admin_Dashboard.png"
if os.path.exists(dash_pic):
    s9.shapes.add_picture(dash_pic, Inches(0.8), Inches(1.5), width=Inches(6.8))

box_adm = s9.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(7.9), Inches(1.5), Inches(4.6), Inches(5.3))
box_adm.fill.solid()
box_adm.fill.fore_color.rgb = WHITE
box_adm.line.color.rgb = GRAY

tf_adm = box_adm.text_frame
tf_adm.word_wrap = True
p = tf_adm.paragraphs[0]
p.text = "Operational Controls"
p.font.size = Pt(16)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(10)

adm_items = [
    "Live KPI Stats: Real-time queries for total doctors, patients, appointments, scheduled, completed, and cancelled counts.",
    "Specialty Analytics Chart: Visual distribution of appointments grouped by medical specialty from Vw_AppointmentsBySpecialty.",
    "Doctor & Schedule Management: Full doctor CRUD, consultation fees, weekly working hours editing, and slot availability toggles.",
    "Specialty Catalog: Dynamic administration of clinical departments, names, descriptions, and icon mappings.",
    "Patient Directory & Management: Searchable patient registry, detail view, patient record editing, and safe deletion.",
    "Appointment Lifecycle: Real-time status updates (Completed, Cancelled) with automatic slot reopening upon cancellation."
]
for ai in adm_items:
    p = tf_adm.add_paragraph()
    p.text = "• " + ai
    p.font.size = Pt(11)
    p.font.color.rgb = DARK_SLATE
    p.space_after = Pt(7)

# ----------------- SLIDE 10: QA, Verification & Results -----------------
s10 = prs.slides.add_slide(blank_layout)
add_header(s10, "Quality Assurance, Concurrency & E2E Validation")

qa_cards = [
    ("Build Quality", "100% Zero-Error Compilation", [
        "ASP.NET Core Web API: Built with 0 Errors, 0 Warnings across all 3 projects.",
        "Angular 21 Frontend: Built with 0 Errors and production optimization.",
        "Oracle PL/SQL: All tables, views, triggers, and package compiled with zero errors."
    ], Inches(0.8), Inches(1.5), Inches(3.6)),
    ("End-to-End Browser QA", "Automated Playwright Suite", [
        "14 comprehensive automated browser test scenarios covering all user & admin journeys.",
        "17 high-resolution screenshots verifying desktop and mobile viewports.",
        "Zero unhandled browser console errors during entire execution."
    ], Inches(4.866), Inches(1.5), Inches(3.6)),
    ("Integration & Stress", "Live Backend & DB Verification", [
        "Pessimistic locking verified under simulated race conditions (HTTP 409 rejection).",
        "JWT token expiration and route protection verified.",
        "Referential integrity verified: foreign key cascades and slot state consistency."
    ], Inches(8.933), Inches(1.5), Inches(3.6))
]

for title, sub, items, left, top, width in qa_cards:
    card = s10.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, left, top, width, Inches(5.3))
    card.fill.solid()
    card.fill.fore_color.rgb = LIGHT_TEAL
    card.line.color.rgb = TEAL

    tf = card.text_frame
    tf.word_wrap = True
    p = tf.paragraphs[0]
    p.text = title
    p.font.size = Pt(16)
    p.font.bold = True
    p.font.color.rgb = NAVY

    p_sub = tf.add_paragraph()
    p_sub.text = sub
    p_sub.font.size = Pt(11)
    p_sub.font.bold = True
    p_sub.font.color.rgb = TEAL
    p_sub.space_after = Pt(12)

    for it in items:
        p_it = tf.add_paragraph()
        p_it.text = "✔ " + it
        p_it.font.size = Pt(11)
        p_it.font.color.rgb = DARK_SLATE
        p_it.space_after = Pt(8)

# ----------------- SLIDE 11: Deliverables & Conclusion -----------------
s11 = prs.slides.add_slide(blank_layout)
add_header(s11, "Project Deliverables & Conclusion")

# Final summary card
f_card = s11.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE, Inches(0.8), Inches(1.5), Inches(11.733), Inches(5.3))
f_card.fill.solid()
f_card.fill.fore_color.rgb = WHITE
f_card.line.color.rgb = TEAL

tf_f = f_card.text_frame
tf_f.word_wrap = True
p = tf_f.paragraphs[0]
p.text = "University Assignment Deliverables Checklist"
p.font.size = Pt(18)
p.font.bold = True
p.font.color.rgb = NAVY
p.space_after = Pt(14)

deliverables = [
    ("1. Oracle Database Script", "database_project/ClinicAppointmentManagementSystem.sql — Complete schema, constraints, views, and PL/SQL package with sample data."),
    ("2. Entity Relationship Diagram (ERD)", "database_project/erdplus.png — Full relational ERD detailing primary keys, foreign keys, and cardinalities."),
    ("3. ASP.NET Core Web API", "Clinic Management System/ — 3-tier clean architecture with Dapper, JWT authentication, and Swagger."),
    ("4. Angular Frontend Application", "Frontend_Clinic/ — Complete Angular 21 Single Page Application with all 14 required public and admin pages."),
    ("5. Main Website Screenshots", "screenshots/ — 17 high-resolution screenshots covering desktop workflows, admin portal, and mobile responsive views."),
    ("6. PowerPoint Presentation", "presentation/Clinic_Appointment_Management_System_Presentation.pptx — Comprehensive technical overview slide deck.")
]

for title, desc in deliverables:
    p_d = tf_f.add_paragraph()
    p_d.text = title + ": " + desc
    p_d.font.size = Pt(11)
    p_d.font.color.rgb = DARK_SLATE
    p_d.space_after = Pt(8)

p_conc = tf_f.add_paragraph()
p_conc.text = "Conclusion: The Clinic Appointment Management System has been fully implemented, integrated, and validated with genuine Oracle 19c database operations, providing a robust, concurrent, and user-friendly healthcare management solution."
p_conc.font.size = Pt(12)
p_conc.font.bold = True
p_conc.font.color.rgb = TEAL
p_conc.space_before = Pt(10)

output_path = "presentation/Clinic_Appointment_Management_System_Presentation.pptx"
prs.save(output_path)
print(f"Presentation saved successfully to {output_path} with {len(prs.slides)} slides.")

