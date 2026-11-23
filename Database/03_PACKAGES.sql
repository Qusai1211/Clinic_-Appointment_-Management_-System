-- ============================================================================
-- Clinic Appointment Management System
-- Tier 1: Database - 03_PACKAGES.sql
-- PL/SQL Package Specifications and Bodies
-- Compatible with Oracle Database 19c / 21c / XE
-- ============================================================================

SET DEFINE OFF;
SET SERVEROUTPUT ON;

--  DDL for Package ADMIN_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "ADMIN_PKG" AS
END admin_pkg;
/

--------------------------------------------------------
--  DDL for Package PKG_ADMINS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_ADMINS_MNG" AS

    PROCEDURE ADD_ADMIN(
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        p_RoleId       IN  NUMBER DEFAULT 1,
        p_Email        IN  VARCHAR2 DEFAULT NULL,
        o_AdminId      OUT NUMBER
    );

    PROCEDURE UPDATE_ADMIN(
        p_AdminId      IN  NUMBER,
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        p_RoleId       IN  NUMBER,
        p_IsActive     IN  NUMBER DEFAULT 1,
        p_Email        IN  VARCHAR2 DEFAULT NULL
    );

    -- حذف منطقي بالرقم التعريفي
    PROCEDURE DELETE_ADMIN_BY_ID(
        p_AdminId      IN  NUMBER
    );

    -- حذف منطقي بالاسم
    PROCEDURE DELETE_ADMIN_BY_NAME(
        p_AdminName    IN  VARCHAR2
    );

    PROCEDURE LOGIN_ADMIN(
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        o_AdminId      OUT NUMBER,
        o_RoleName     OUT VARCHAR2,
        o_IsSuccess    OUT NUMBER
    );

    PROCEDURE GET_ALL_ADMINS(
        o_Cursor       OUT SYS_REFCURSOR
    );

    PROCEDURE GET_ADMIN_BY_ID(
        p_AdminId      IN  NUMBER,
        o_Cursor       OUT SYS_REFCURSOR
    );

END PKG_ADMINS_MNG;
/

--------------------------------------------------------
--  DDL for Package PKG_CLINIC_OPERATIONS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_CLINIC_OPERATIONS" AS

    -- تسجيل مريض جديد أو تحديث بياناته إذا كان مسجلاً، وإرجاع معرفه o_PatientId
    PROCEDURE REGISTER_PATIENT(
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE,
        o_PatientId   OUT NUMBER
    );

    -- حجز موعد وتوليد رقم مرجعي تلقائي وإغلاق الفترة الزمنية
    PROCEDURE BOOK_APPOINTMENT(
        p_DoctorId        IN  NUMBER,
        p_PatientId       IN  NUMBER,
        p_SlotId          IN  NUMBER,
        p_AppointmentDate IN  DATE,
        p_AppointmentTime IN  VARCHAR2,
        o_AppointmentId   OUT NUMBER,
        o_ReferenceNumber OUT VARCHAR2
    );

    -- إلغاء موعد وإعادة فتح الفترة الزمنية للحجز مجدداً
    PROCEDURE CANCEL_APPOINTMENT(
        p_AppointmentId IN NUMBER
    );

    -- تحديث حالة الموعد (Scheduled / Completed / Cancelled)
    PROCEDURE UPDATE_APPOINTMENT_STATUS(
        p_AppointmentId IN NUMBER,
        p_Status        IN VARCHAR2
    );

    -- جلب تفاصيل موعد واحد حسب رقمه التعريفي ID
    PROCEDURE GET_APPOINTMENT_BY_ID(
        p_AppointmentId IN  NUMBER,
        o_Cursor        OUT SYS_REFCURSOR
    );

    -- جلب تفاصيل كافة المواعيد مع بيانات المريض والطبيب والتخصص
    PROCEDURE GET_ALL_APPOINTMENTS_DETAILS(
        o_Cursor OUT SYS_REFCURSOR
    );

    -- جلب تفاصيل الأطباء النشطين مع أسماء تخصصاتهم
    PROCEDURE GET_DOCTORS_WITH_SPECIALTY(
        o_Cursor OUT SYS_REFCURSOR
    );

END PKG_CLINIC_OPERATIONS;
/

--------------------------------------------------------
--  DDL for Package PKG_CLINIC_SEARCH
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_CLINIC_SEARCH" AS

    -- 1. فلترة والبحث في الأطباء (بالاسم، التخصص، حد الكشفية، والترتيب بالاسم أو التخصص)
    PROCEDURE SEARCH_DOCTORS(
        p_SearchName  IN  VARCHAR2 DEFAULT NULL,
        p_SpecialtyId IN  NUMBER   DEFAULT NULL,
        p_MaxFee      IN  NUMBER   DEFAULT NULL,
        p_SortBy      IN  VARCHAR2 DEFAULT 'NAME', -- 'NAME' أو 'SPECIALTY'
        o_Cursor      OUT SYS_REFCURSOR
    );

    -- 2. فلترة والبحث في المواعيد (باسم المريض أو الطبيب، الحالة، الطبيب، ونطاق التواريخ)
    PROCEDURE FILTER_APPOINTMENTS(
        p_SearchTerm IN  VARCHAR2 DEFAULT NULL,
        p_Status     IN  VARCHAR2 DEFAULT NULL,
        p_DoctorId   IN  NUMBER   DEFAULT NULL,
        p_FromDate   IN  DATE     DEFAULT NULL,
        p_ToDate     IN  DATE     DEFAULT NULL,
        o_Cursor     OUT SYS_REFCURSOR
    );

    -- 3. فلترة والبحث في المرضى (بالاسم، رقم الهاتف، أو البريد)
    PROCEDURE SEARCH_PATIENTS(
        p_SearchTerm IN  VARCHAR2 DEFAULT NULL,
        o_Cursor     OUT SYS_REFCURSOR
    );

END PKG_CLINIC_SEARCH;
/

--------------------------------------------------------
--  DDL for Package PKG_DOCTORS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_DOCTORS_MNG" AS

    PROCEDURE ADD_DOCTOR(
        p_FullName        IN  VARCHAR2,
        p_Qualifications  IN  VARCHAR2,
        p_ConsultationFee IN  NUMBER,
        p_ProfileImageUrl IN  VARCHAR2,
        p_IsFeatured      IN  NUMBER,
        p_SpecialtyId     IN  NUMBER,
        o_DoctorId        OUT NUMBER
    );

    PROCEDURE UPDATE_DOCTOR(
        p_DoctorId        IN  NUMBER,
        p_FullName        IN  VARCHAR2,
        p_Qualifications  IN  VARCHAR2,
        p_ConsultationFee IN  NUMBER,
        p_ProfileImageUrl IN  VARCHAR2,
        p_IsFeatured      IN  NUMBER,
        p_SpecialtyId     IN  NUMBER
    );

    PROCEDURE DELETE_DOCTOR(
        p_DoctorId        IN  NUMBER
    );

    PROCEDURE GET_ALL_DOCTORS(
        o_Cursor          OUT SYS_REFCURSOR
    );

    PROCEDURE GET_DOCTOR_BY_ID(
        p_DoctorId        IN  NUMBER,
        o_Cursor          OUT SYS_REFCURSOR
    );

END PKG_DOCTORS_MNG;
/

--------------------------------------------------------
--  DDL for Package PKG_PATIENTS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_PATIENTS_MNG" AS

    -- إضافة مريض جديد وإرجاع رقمه التعريفي o_PatientId
    PROCEDURE ADD_PATIENT(
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE,
        o_PatientId   OUT NUMBER
    );

    -- تحديث بيانات مريض نشط
    PROCEDURE UPDATE_PATIENT(
        p_PatientId   IN  NUMBER,
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE
    );

    -- حذف مريض منطقياً (تعطيله وليس حذفه نهائياً)
    PROCEDURE DELETE_PATIENT(
        p_PatientId   IN  NUMBER
    );

    -- جلب جميع المرضى النشطين
    PROCEDURE GET_ALL_PATIENTS(
        o_Cursor      OUT SYS_REFCURSOR
    );

    -- جلب بيانات مريض محدد حسب رقمه التعريفي ID
    PROCEDURE GET_PATIENT_BY_ID(
        p_PatientId   IN  NUMBER,
        o_Cursor      OUT SYS_REFCURSOR
    );

END PKG_PATIENTS_MNG;
/

--------------------------------------------------------
--  DDL for Package PKG_SLOTS_WORKING_HOURS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_SLOTS_WORKING_HOURS" AS

    -- إضافة يوم وساعات دوام لطبيب
    PROCEDURE ADD_WORKING_HOURS(
        p_DayOfWeek IN  VARCHAR2,
        p_StartTime IN  VARCHAR2,
        p_EndTime   IN  VARCHAR2,
        p_DoctorId  IN  NUMBER,
        o_WorkId    OUT NUMBER
    );

    -- تعديل وقت دوام طبيب
    PROCEDURE UPDATE_WORKING_HOURS(
        p_WorkingHoursId IN NUMBER,
        p_DayOfWeek      IN VARCHAR2,
        p_StartTime      IN VARCHAR2,
        p_EndTime        IN VARCHAR2,
        p_DoctorId       IN NUMBER
    );

    -- حذف وقت دوام طبيب
    PROCEDURE DELETE_WORKING_HOURS(
        p_WorkingHoursId IN NUMBER
    );

    -- جلب أوقات دوام طبيب محدد حسب DoctorId
    PROCEDURE GET_WORKING_HOURS_BY_DOCTOR(
        p_DoctorId IN  NUMBER,
        o_Cursor   OUT SYS_REFCURSOR
    );

    -- إضافة فترة حجز جديدة لطبيب
    PROCEDURE ADD_APPOINTMENT_SLOT(
        p_SlotDate  IN  DATE,
        p_SlotTime  IN  VARCHAR2,
        p_DoctorId  IN  NUMBER,
        o_SlotId    OUT NUMBER
    );

    -- تعديل فترة حجز أو حالتها (متاحة/محجوزة)
    PROCEDURE UPDATE_APPOINTMENT_SLOT(
        p_SlotId      IN NUMBER,
        p_SlotDate    IN DATE,
        p_SlotTime    IN VARCHAR2,
        p_IsAvailable IN NUMBER,
        p_DoctorId    IN NUMBER
    );

    -- حذف فترة حجز
    PROCEDURE DELETE_APPOINTMENT_SLOT(
        p_SlotId    IN  NUMBER
    );

    -- جلب الفترات الزمنية المتاحة للحجز لطبيب معين من تاريخ محدد فصاعداً
    PROCEDURE GET_AVAILABLE_SLOTS_BY_DOCTOR(
        p_DoctorId IN  NUMBER,
        p_FromDate IN  DATE DEFAULT TRUNC(SYSDATE),
        o_Cursor   OUT SYS_REFCURSOR
    );

END PKG_SLOTS_WORKING_HOURS;
/

--------------------------------------------------------
--  DDL for Package PKG_SPECIALTIES_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE "PKG_SPECIALTIES_MNG" AS

    -- إضافة تخصص طبي جديد
    PROCEDURE ADD_SPECIALTY(
        p_SpecialtyName IN  VARCHAR2,
        p_Description   IN  VARCHAR2,
        o_SpecialtyId   OUT NUMBER
    );

    -- تعديل بيانات تخصص موجود
    PROCEDURE UPDATE_SPECIALTY(
        p_SpecialtyId   IN  NUMBER,
        p_SpecialtyName IN  VARCHAR2,
        p_Description   IN  VARCHAR2
    );

    -- حذف تخصص حسب رقمه التعريفي ID
    PROCEDURE DELETE_SPECIALTY(
        p_SpecialtyId   IN  NUMBER
    );

    -- جلب جميع التخصصات الطبية
    PROCEDURE GET_ALL_SPECIALTIES(
        o_Cursor        OUT SYS_REFCURSOR
    );

    -- جلب تخصص محدد حسب رقمه التعريفي ID
    PROCEDURE GET_SPECIALTY_BY_ID(
        p_SpecialtyId   IN  NUMBER,
        o_Cursor        OUT SYS_REFCURSOR
    );

END PKG_SPECIALTIES_MNG;
/

--------------------------------------------------------
--  DDL for Package Body ADMIN_PKG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "ADMIN_PKG" AS
  
END admin_pkg;
/

--------------------------------------------------------
--  DDL for Package Body PKG_ADMINS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_ADMINS_MNG" AS

    -- إضافة مشرف جديد
    PROCEDURE ADD_ADMIN(
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        p_RoleId       IN  NUMBER DEFAULT 1,
        p_Email        IN  VARCHAR2 DEFAULT NULL,
        o_AdminId      OUT NUMBER
    ) IS
    BEGIN
        INSERT INTO Admins (AdminName, PasswordHash, RoleId, IsActive, Email)
        VALUES (TRIM(p_AdminName), p_PasswordHash, NVL(p_RoleId, 1), 1, TRIM(p_Email))
        RETURNING AdminId INTO o_AdminId;

        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20060, 'Admin username already exists.');
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20061, 'Error adding admin: ' || SQLERRM);
    END ADD_ADMIN;

    -- تحديث بيانات مشرف (القيم الفارغة NULL ما بتغيّر الحقل)
    PROCEDURE UPDATE_ADMIN(
        p_AdminId      IN  NUMBER,
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        p_RoleId       IN  NUMBER,
        p_IsActive     IN  NUMBER DEFAULT 1,
        p_Email        IN  VARCHAR2 DEFAULT NULL
    ) IS
    BEGIN
        UPDATE Admins
        SET AdminName    = NVL(TRIM(p_AdminName), AdminName),
            PasswordHash = NVL(p_PasswordHash, PasswordHash),
            RoleId       = NVL(p_RoleId, RoleId),
            IsActive     = NVL(p_IsActive, IsActive),
            Email        = COALESCE(TRIM(p_Email), Email)
        WHERE AdminId = p_AdminId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20062, 'Admin not found.');
        END IF;

        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20060, 'Admin username already exists.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20063, 'Error updating admin: ' || SQLERRM);
    END UPDATE_ADMIN;

    -- حذف منطقي بالرقم
    PROCEDURE DELETE_ADMIN_BY_ID(p_AdminId IN NUMBER) IS
    BEGIN
        UPDATE Admins
        SET IsActive = 0
        WHERE AdminId = p_AdminId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20064, 'Admin not found.');
        END IF;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20065, 'Error deleting admin: ' || SQLERRM);
    END DELETE_ADMIN_BY_ID;

    -- حذف منطقي بالاسم
    PROCEDURE DELETE_ADMIN_BY_NAME(p_AdminName IN VARCHAR2) IS
    BEGIN
        UPDATE Admins
        SET IsActive = 0
        WHERE AdminName = TRIM(p_AdminName);

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20064, 'Admin not found.');
        END IF;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20065, 'Error deleting admin: ' || SQLERRM);
    END DELETE_ADMIN_BY_NAME;

    -- تسجيل الدخول: بيرجع o_IsSuccess = 0 عند الفشل بدون رمي خطأ
    PROCEDURE LOGIN_ADMIN(
        p_AdminName    IN  VARCHAR2,
        p_PasswordHash IN  VARCHAR2,
        o_AdminId      OUT NUMBER,
        o_RoleName     OUT VARCHAR2,
        o_IsSuccess    OUT NUMBER
    ) IS
    BEGIN
        SELECT a.AdminId, r.RoleName
        INTO o_AdminId, o_RoleName
        FROM Admins a
        LEFT JOIN Roles r ON a.RoleId = r.RoleId
        WHERE a.AdminName = TRIM(p_AdminName)
          AND a.PasswordHash = p_PasswordHash
          AND a.IsActive = 1;

        o_IsSuccess := 1;
    EXCEPTION
        WHEN OTHERS THEN
            o_AdminId   := NULL;
            o_RoleName  := NULL;
            o_IsSuccess := 0;
    END LOGIN_ADMIN;

    -- جلب كل المشرفين النشطين
    PROCEDURE GET_ALL_ADMINS(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT a.AdminId, a.AdminName, a.RoleId, r.RoleName, a.IsActive, a.Email
            FROM Admins a
            LEFT JOIN Roles r ON a.RoleId = r.RoleId
            WHERE a.IsActive = 1
            ORDER BY a.AdminId;
    END GET_ALL_ADMINS;

    -- جلب مشرف واحد حسب الرقم
    PROCEDURE GET_ADMIN_BY_ID(
        p_AdminId IN  NUMBER,
        o_Cursor  OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT a.AdminId, a.AdminName, a.RoleId, r.RoleName, a.IsActive, a.Email
            FROM Admins a
            LEFT JOIN Roles r ON a.RoleId = r.RoleId
            WHERE a.AdminId = p_AdminId AND a.IsActive = 1;
    END GET_ADMIN_BY_ID;

END PKG_ADMINS_MNG;
/

--------------------------------------------------------
--  DDL for Package Body PKG_CLINIC_OPERATIONS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_CLINIC_OPERATIONS" AS

    -- تسجيل مريض جديد أو مطابقة المريض الحالي وإعادة تفعيله
    PROCEDURE REGISTER_PATIENT(
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE,
        o_PatientId   OUT NUMBER
    ) IS
        v_ExistingId NUMBER;
    BEGIN
        BEGIN
            SELECT PatientId INTO v_ExistingId
            FROM Patients
            WHERE LOWER(Email) = LOWER(TRIM(p_Email))
              AND TRIM(PhoneNumber) = TRIM(p_PhoneNumber)
            FETCH FIRST 1 ROW ONLY;

            UPDATE Patients
            SET FullName = TRIM(p_FullName),
                DateOfBirth = TRUNC(p_DateOfBirth),
                IsActive = 1
            WHERE PatientId = v_ExistingId;

            o_PatientId := v_ExistingId;
            COMMIT;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                INSERT INTO Patients (FullName, PhoneNumber, Email, DateOfBirth, IsActive)
                VALUES (TRIM(p_FullName), TRIM(p_PhoneNumber), LOWER(TRIM(p_Email)), TRUNC(p_DateOfBirth), 1)
                RETURNING PatientId INTO o_PatientId;
                COMMIT;
        END;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20008, 'Email or phone number is already registered to another patient.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20001, 'Error registering patient: ' || SQLERRM);
    END REGISTER_PATIENT;

    -- حجز موعد مع قفل الفترة الزمنية (FOR UPDATE) لمنع الحجز المزدوج
    PROCEDURE BOOK_APPOINTMENT(
        p_DoctorId        IN  NUMBER,
        p_PatientId       IN  NUMBER,
        p_SlotId          IN  NUMBER,
        p_AppointmentDate IN  DATE,
        p_AppointmentTime IN  VARCHAR2,
        o_AppointmentId   OUT NUMBER,
        o_ReferenceNumber OUT VARCHAR2
    ) IS
        v_IsAvailable  NUMBER;
        v_SlotDoctorId NUMBER;
    BEGIN
        SELECT IsAvailable, DoctorId INTO v_IsAvailable, v_SlotDoctorId
        FROM AppointmentSlots
        WHERE SlotId = p_SlotId
        FOR UPDATE;

        IF v_SlotDoctorId != p_DoctorId THEN
            RAISE_APPLICATION_ERROR(-20009, 'Selected time slot does not belong to the specified doctor.');
        END IF;

        IF v_IsAvailable = 1 THEN
            o_ReferenceNumber := 'REF-' || TO_CHAR(SYSDATE, 'YYYYMMDD') || '-' || seq_appt_ref.NEXTVAL;

            INSERT INTO Appointments (ReferenceNumber, AppointmentDate, AppointmentTime, Status, DoctorId, SlotId, PatientId)
            VALUES (o_ReferenceNumber, TRUNC(p_AppointmentDate), UPPER(TRIM(p_AppointmentTime)), 'Scheduled', p_DoctorId, p_SlotId, p_PatientId)
            RETURNING AppointmentId INTO o_AppointmentId;

            UPDATE AppointmentSlots
            SET IsAvailable = 0
            WHERE SlotId = p_SlotId;

            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20002, 'This time slot is not available or has already been booked.');
        END IF;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20003, 'Appointment slot not found.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20004, 'Error booking appointment: ' || SQLERRM);
    END BOOK_APPOINTMENT;

    -- إلغاء موعد وإرجاع حالة الفترة الزمنية إلى متاحة (IsAvailable = 1)
    PROCEDURE CANCEL_APPOINTMENT(p_AppointmentId IN NUMBER) IS
        v_SlotId NUMBER;
    BEGIN
        UPDATE Appointments
        SET Status = 'Cancelled'
        WHERE AppointmentId = p_AppointmentId
        RETURNING SlotId INTO v_SlotId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20005, 'Appointment not found.');
        END IF;

        IF v_SlotId IS NOT NULL THEN
            UPDATE AppointmentSlots SET IsAvailable = 1 WHERE SlotId = v_SlotId;
        END IF;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20006, 'Error cancelling appointment: ' || SQLERRM);
    END CANCEL_APPOINTMENT;

    -- تحديث حالة الموعد وإعادة فتح الفترة إذا تم تحويل الحالة إلى Cancelled
    PROCEDURE UPDATE_APPOINTMENT_STATUS(p_AppointmentId IN NUMBER, p_Status IN VARCHAR2) IS
        v_SlotId NUMBER;
    BEGIN
        UPDATE Appointments
        SET Status = p_Status
        WHERE AppointmentId = p_AppointmentId
        RETURNING SlotId INTO v_SlotId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20005, 'Appointment not found.');
        END IF;

        IF p_Status = 'Cancelled' AND v_SlotId IS NOT NULL THEN
            UPDATE AppointmentSlots SET IsAvailable = 1 WHERE SlotId = v_SlotId;
        END IF;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20007, 'Error updating appointment status: ' || SQLERRM);
    END UPDATE_APPOINTMENT_STATUS;

    -- جلب تفاصيل موعد محدد حسب AppointmentId
    PROCEDURE GET_APPOINTMENT_BY_ID(
        p_AppointmentId IN  NUMBER,
        o_Cursor        OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT
                a.AppointmentId, a.ReferenceNumber, a.AppointmentDate, a.AppointmentTime, a.Status,
                a.DoctorId, a.PatientId, a.SlotId,
                p.FullName AS PatientName, p.PhoneNumber AS PatientPhone, p.Email AS PatientEmail,
                d.FullName AS DoctorName, d.ConsultationFee, s.SpecialtyName
            FROM Appointments a
            JOIN Patients p ON a.PatientId = p.PatientId
            LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE a.AppointmentId = p_AppointmentId;
    END GET_APPOINTMENT_BY_ID;

    -- جلب كافة المواعيد مع تفاصيل المريض والطبيب والتخصص
    PROCEDURE GET_ALL_APPOINTMENTS_DETAILS(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT
                a.AppointmentId, a.ReferenceNumber, a.AppointmentDate, a.AppointmentTime, a.Status,
                a.DoctorId, a.PatientId, a.SlotId,
                p.FullName AS PatientName, p.PhoneNumber AS PatientPhone,
                d.FullName AS DoctorName, s.SpecialtyName
            FROM Appointments a
            JOIN Patients p ON a.PatientId = p.PatientId
            LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            ORDER BY a.AppointmentDate DESC, a.AppointmentTime ASC;
    END GET_ALL_APPOINTMENTS_DETAILS;

    -- جلب الأطباء النشطين مع اسم التخصص
    PROCEDURE GET_DOCTORS_WITH_SPECIALTY(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT
                d.DoctorId, d.FullName, d.Qualifications, d.ConsultationFee,
                d.ProfileImageUrl, d.IsFeatured, d.SpecialtyId, s.SpecialtyName
            FROM Doctors d
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE d.IsActive = 1
            ORDER BY d.DoctorId;
    END GET_DOCTORS_WITH_SPECIALTY;

END PKG_CLINIC_OPERATIONS;
/

--------------------------------------------------------
--  DDL for Package Body PKG_CLINIC_SEARCH
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_CLINIC_SEARCH" AS

    PROCEDURE SEARCH_DOCTORS(
        p_SearchName  IN  VARCHAR2 DEFAULT NULL,
        p_SpecialtyId IN  NUMBER   DEFAULT NULL,
        p_MaxFee      IN  NUMBER   DEFAULT NULL,
        p_SortBy      IN  VARCHAR2 DEFAULT 'NAME',
        o_Cursor      OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT 
                d.DoctorId,
                d.FullName,
                d.Qualifications,
                d.ConsultationFee,
                d.ProfileImageUrl,
                d.IsFeatured,
                s.SpecialtyId,
                s.SpecialtyName
            FROM Doctors d
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE d.IsActive = 1
              AND (p_SearchName IS NULL OR LOWER(d.FullName) LIKE '%' || LOWER(TRIM(p_SearchName)) || '%')
              AND (p_SpecialtyId IS NULL OR d.SpecialtyId = p_SpecialtyId)
              AND (p_MaxFee IS NULL OR d.ConsultationFee <= p_MaxFee)
            ORDER BY
                CASE WHEN UPPER(p_SortBy) = 'SPECIALTY' THEN s.SpecialtyName ELSE d.FullName END ASC,
                d.FullName ASC;
    END SEARCH_DOCTORS;

    PROCEDURE FILTER_APPOINTMENTS(
        p_SearchTerm IN  VARCHAR2 DEFAULT NULL,
        p_Status     IN  VARCHAR2 DEFAULT NULL,
        p_DoctorId   IN  NUMBER   DEFAULT NULL,
        p_FromDate   IN  DATE     DEFAULT NULL,
        p_ToDate     IN  DATE     DEFAULT NULL,
        o_Cursor     OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT 
                a.AppointmentId,
                a.ReferenceNumber,
                a.AppointmentDate,
                a.AppointmentTime,
                a.Status,
                a.DoctorId,
                a.PatientId,
                a.SlotId,
                p.FullName AS PatientName,
                p.PhoneNumber,
                d.FullName AS DoctorName,
                s.SpecialtyName
            FROM Appointments a
            JOIN Patients p ON a.PatientId = p.PatientId
            LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE (p_SearchTerm IS NULL
                   OR LOWER(p.FullName) LIKE '%' || LOWER(TRIM(p_SearchTerm)) || '%'
                   OR LOWER(d.FullName) LIKE '%' || LOWER(TRIM(p_SearchTerm)) || '%')
              AND (p_Status IS NULL OR LOWER(a.Status) = LOWER(TRIM(p_Status)))
              AND (p_DoctorId IS NULL OR a.DoctorId = p_DoctorId)
              AND (p_FromDate IS NULL OR TRUNC(a.AppointmentDate) >= TRUNC(p_FromDate))
              AND (p_ToDate IS NULL OR TRUNC(a.AppointmentDate) <= TRUNC(p_ToDate))
            ORDER BY a.AppointmentDate DESC, a.AppointmentTime ASC;
    END FILTER_APPOINTMENTS;

    PROCEDURE SEARCH_PATIENTS(
        p_SearchTerm IN  VARCHAR2 DEFAULT NULL,
        o_Cursor     OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT 
                PatientId, 
                FullName, 
                PhoneNumber, 
                Email, 
                DateOfBirth
            FROM Patients
            WHERE IsActive = 1
              AND (p_SearchTerm IS NULL 
               OR LOWER(FullName) LIKE '%' || LOWER(TRIM(p_SearchTerm)) || '%'
               OR PhoneNumber LIKE '%' || TRIM(p_SearchTerm) || '%'
               OR LOWER(Email) LIKE '%' || LOWER(TRIM(p_SearchTerm)) || '%')
            ORDER BY FullName;
    END SEARCH_PATIENTS;

END PKG_CLINIC_SEARCH;
/

--------------------------------------------------------
--  DDL for Package Body PKG_DOCTORS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_DOCTORS_MNG" AS

    PROCEDURE ADD_DOCTOR(
        p_FullName        IN  VARCHAR2,
        p_Qualifications  IN  VARCHAR2,
        p_ConsultationFee IN  NUMBER,
        p_ProfileImageUrl IN  VARCHAR2,
        p_IsFeatured      IN  NUMBER,
        p_SpecialtyId     IN  NUMBER,
        o_DoctorId        OUT NUMBER
    ) IS
    BEGIN
        INSERT INTO Doctors (FullName, Qualifications, ConsultationFee, ProfileImageUrl, IsFeatured, IsActive, SpecialtyId)
        VALUES (TRIM(p_FullName), p_Qualifications, p_ConsultationFee, p_ProfileImageUrl, NVL(p_IsFeatured, 0), 1, p_SpecialtyId)
        RETURNING DoctorId INTO o_DoctorId;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20010, 'Error adding doctor: ' || SQLERRM);
    END ADD_DOCTOR;

    PROCEDURE UPDATE_DOCTOR(
        p_DoctorId        IN  NUMBER,
        p_FullName        IN  VARCHAR2,
        p_Qualifications  IN  VARCHAR2,
        p_ConsultationFee IN  NUMBER,
        p_ProfileImageUrl IN  VARCHAR2,
        p_IsFeatured      IN  NUMBER,
        p_SpecialtyId     IN  NUMBER
    ) IS
    BEGIN
        UPDATE Doctors
        SET FullName = TRIM(p_FullName),
            Qualifications = p_Qualifications,
            ConsultationFee = p_ConsultationFee,
            ProfileImageUrl = p_ProfileImageUrl,
            IsFeatured = NVL(p_IsFeatured, 0),
            SpecialtyId = p_SpecialtyId
        WHERE DoctorId = p_DoctorId AND IsActive = 1;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20011, 'Doctor not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20012, 'Error updating doctor: ' || SQLERRM);
    END UPDATE_DOCTOR;

    PROCEDURE DELETE_DOCTOR(p_DoctorId IN NUMBER) IS
        v_ScheduledCount NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_ScheduledCount
        FROM Appointments
        WHERE DoctorId = p_DoctorId AND Status = 'Scheduled';

        IF v_ScheduledCount > 0 THEN
            RAISE_APPLICATION_ERROR(-20015, 'Cannot delete doctor with scheduled appointments.');
        END IF;

        UPDATE Doctors SET IsActive = 0 WHERE DoctorId = p_DoctorId;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20013, 'Doctor not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20014, 'Error deleting doctor: ' || SQLERRM);
    END DELETE_DOCTOR;

    PROCEDURE GET_ALL_DOCTORS(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT
                d.DoctorId,
                d.FullName,
                d.Qualifications,
                d.ConsultationFee,
                d.ProfileImageUrl,
                d.IsFeatured,
                d.IsActive,
                d.SpecialtyId,
                s.SpecialtyName
            FROM Doctors d
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE d.IsActive = 1
            ORDER BY d.DoctorId;
    END GET_ALL_DOCTORS;

    PROCEDURE GET_DOCTOR_BY_ID(
        p_DoctorId IN  NUMBER,
        o_Cursor   OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT
                d.DoctorId,
                d.FullName,
                d.Qualifications,
                d.ConsultationFee,
                d.ProfileImageUrl,
                d.IsFeatured,
                d.IsActive,
                d.SpecialtyId,
                s.SpecialtyName,
                s.Description AS SpecialtyDescription
            FROM Doctors d
            LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
            WHERE d.DoctorId = p_DoctorId AND d.IsActive = 1;
    END GET_DOCTOR_BY_ID;

END PKG_DOCTORS_MNG;
/

--------------------------------------------------------
--  DDL for Package Body PKG_PATIENTS_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_PATIENTS_MNG" AS

    -- إضافة مريض جديد مع توحيد صيغة البريد وإزالة الفراغات الزائدة
    PROCEDURE ADD_PATIENT(
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE,
        o_PatientId   OUT NUMBER
    ) IS
    BEGIN
        INSERT INTO Patients (FullName, PhoneNumber, Email, DateOfBirth, IsActive)
        VALUES (TRIM(p_FullName), TRIM(p_PhoneNumber), LOWER(TRIM(p_Email)), TRUNC(p_DateOfBirth), 1)
        RETURNING PatientId INTO o_PatientId;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20020, 'Phone number or email is already in use.');
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20021, 'Error adding patient: ' || SQLERRM);
    END ADD_PATIENT;

    -- تحديث بيانات مريض موجود ونشط
    PROCEDURE UPDATE_PATIENT(
        p_PatientId   IN  NUMBER,
        p_FullName    IN  VARCHAR2,
        p_PhoneNumber IN  VARCHAR2,
        p_Email       IN  VARCHAR2,
        p_DateOfBirth IN  DATE
    ) IS
    BEGIN
        UPDATE Patients
        SET FullName = TRIM(p_FullName),
            PhoneNumber = TRIM(p_PhoneNumber),
            Email = LOWER(TRIM(p_Email)),
            DateOfBirth = TRUNC(p_DateOfBirth)
        WHERE PatientId = p_PatientId AND IsActive = 1;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20022, 'Patient not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20023, 'Phone number or email is already in use.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20024, 'Error updating patient: ' || SQLERRM);
    END UPDATE_PATIENT;

    -- حذف مريض منطقياً بعد التأكد من عدم وجود مواعيد مجدولة (Scheduled) له
    PROCEDURE DELETE_PATIENT(p_PatientId IN NUMBER) IS
        v_ScheduledCount NUMBER;
    BEGIN
        -- فحص وجود مواعيد مجدولة للمريض قبل التعطيل
        SELECT COUNT(*) INTO v_ScheduledCount
        FROM Appointments
        WHERE PatientId = p_PatientId AND Status = 'Scheduled';

        IF v_ScheduledCount > 0 THEN
            RAISE_APPLICATION_ERROR(-20027, 'Cannot delete patient with scheduled appointments.');
        END IF;

        -- تعطيل حساب المريض (حذف منطقي)
        UPDATE Patients SET IsActive = 0 WHERE PatientId = p_PatientId;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20025, 'Patient not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20026, 'Error deleting patient: ' || SQLERRM);
    END DELETE_PATIENT;

    -- إرجاع كافة المرضى النشطين مرتبين أبجدياً بالاسم
    PROCEDURE GET_ALL_PATIENTS(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT PatientId, FullName, PhoneNumber, Email, DateOfBirth, IsActive
            FROM Patients
            WHERE IsActive = 1
            ORDER BY FullName;
    END GET_ALL_PATIENTS;

    -- إرجاع بيانات مريض واحد حسب ID إذا كان نشطاً
    PROCEDURE GET_PATIENT_BY_ID(p_PatientId IN NUMBER, o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT PatientId, FullName, PhoneNumber, Email, DateOfBirth, IsActive
            FROM Patients
            WHERE PatientId = p_PatientId AND IsActive = 1;
    END GET_PATIENT_BY_ID;

END PKG_PATIENTS_MNG;
/

--------------------------------------------------------
--  DDL for Package Body PKG_SLOTS_WORKING_HOURS
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_SLOTS_WORKING_HOURS" AS

    -- إضافة سجل دوام جديد للطبيب
    PROCEDURE ADD_WORKING_HOURS(p_DayOfWeek IN VARCHAR2, p_StartTime IN VARCHAR2, p_EndTime IN VARCHAR2, p_DoctorId IN NUMBER, o_WorkId OUT NUMBER) IS
    BEGIN
        INSERT INTO DoctorWorkingHours (DayOfWeek, StartTime, EndTime, DoctorId)
        VALUES (TRIM(p_DayOfWeek), UPPER(TRIM(p_StartTime)), UPPER(TRIM(p_EndTime)), p_DoctorId)
        RETURNING WorkingHoursId INTO o_WorkId;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20040, 'Error adding working hours: ' || SQLERRM);
    END ADD_WORKING_HOURS;

    -- تحديث بيانات دوام الطبيب
    PROCEDURE UPDATE_WORKING_HOURS(p_WorkingHoursId IN NUMBER, p_DayOfWeek IN VARCHAR2, p_StartTime IN VARCHAR2, p_EndTime IN VARCHAR2, p_DoctorId IN NUMBER) IS
    BEGIN
        UPDATE DoctorWorkingHours
        SET DayOfWeek = TRIM(p_DayOfWeek),
            StartTime = UPPER(TRIM(p_StartTime)),
            EndTime = UPPER(TRIM(p_EndTime)),
            DoctorId = p_DoctorId
        WHERE WorkingHoursId = p_WorkingHoursId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20046, 'Working hours record not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20047, 'Error updating working hours: ' || SQLERRM);
    END UPDATE_WORKING_HOURS;

    -- حذف سجل دوام
    PROCEDURE DELETE_WORKING_HOURS(p_WorkingHoursId IN NUMBER) IS
    BEGIN
        DELETE FROM DoctorWorkingHours WHERE WorkingHoursId = p_WorkingHoursId;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20041, 'Working hours record not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20042, 'Error deleting working hours: ' || SQLERRM);
    END DELETE_WORKING_HOURS;

    -- جلب أيام وساعات عمل الطبيب
    PROCEDURE GET_WORKING_HOURS_BY_DOCTOR(p_DoctorId IN NUMBER, o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT WorkingHoursId, DayOfWeek, StartTime, EndTime, DoctorId
            FROM DoctorWorkingHours
            WHERE DoctorId = p_DoctorId
            ORDER BY WorkingHoursId;
    END GET_WORKING_HOURS_BY_DOCTOR;

    -- إضافة فترة حجز وتكون متاحة افتراضياً (IsAvailable = 1)
    PROCEDURE ADD_APPOINTMENT_SLOT(p_SlotDate IN DATE, p_SlotTime IN VARCHAR2, p_DoctorId IN NUMBER, o_SlotId OUT NUMBER) IS
    BEGIN
        INSERT INTO AppointmentSlots (SlotDate, SlotTime, IsAvailable, DoctorId)
        VALUES (TRUNC(p_SlotDate), UPPER(TRIM(p_SlotTime)), 1, p_DoctorId)
        RETURNING SlotId INTO o_SlotId;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20048, 'This time slot already exists for this doctor.');
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20043, 'Error adding appointment slot: ' || SQLERRM);
    END ADD_APPOINTMENT_SLOT;

    -- تعديل فترة حجز موجودة
    PROCEDURE UPDATE_APPOINTMENT_SLOT(p_SlotId IN NUMBER, p_SlotDate IN DATE, p_SlotTime IN VARCHAR2, p_IsAvailable IN NUMBER, p_DoctorId IN NUMBER) IS
    BEGIN
        UPDATE AppointmentSlots
        SET SlotDate = TRUNC(p_SlotDate),
            SlotTime = UPPER(TRIM(p_SlotTime)),
            IsAvailable = NVL(p_IsAvailable, 1),
            DoctorId = p_DoctorId
        WHERE SlotId = p_SlotId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20049, 'Appointment slot not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20048, 'This time slot already exists for this doctor.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20052, 'Error updating appointment slot: ' || SQLERRM);
    END UPDATE_APPOINTMENT_SLOT;

    -- حذف فترة حجز
    PROCEDURE DELETE_APPOINTMENT_SLOT(p_SlotId IN NUMBER) IS
    BEGIN
        DELETE FROM AppointmentSlots WHERE SlotId = p_SlotId;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20044, 'Appointment slot not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20045, 'Error deleting appointment slot: ' || SQLERRM);
    END DELETE_APPOINTMENT_SLOT;

    -- جلب الفترات المتاحة فقط (IsAvailable = 1) للطبيب ابتداءً من تاريخ البداية
    PROCEDURE GET_AVAILABLE_SLOTS_BY_DOCTOR(p_DoctorId IN NUMBER, p_FromDate IN DATE DEFAULT TRUNC(SYSDATE), o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT SlotId, SlotDate, SlotTime, IsAvailable, DoctorId
            FROM AppointmentSlots
            WHERE DoctorId = p_DoctorId
              AND IsAvailable = 1
              AND TRUNC(SlotDate) >= TRUNC(NVL(p_FromDate, SYSDATE))
            ORDER BY SlotDate ASC, SlotTime ASC;
    END GET_AVAILABLE_SLOTS_BY_DOCTOR;

END PKG_SLOTS_WORKING_HOURS;
/

--------------------------------------------------------
--  DDL for Package Body PKG_SPECIALTIES_MNG
--------------------------------------------------------

  CREATE OR REPLACE EDITIONABLE PACKAGE BODY "PKG_SPECIALTIES_MNG" AS

    -- إضافة تخصص جديد مع منع تكرار الاسم
    PROCEDURE ADD_SPECIALTY(p_SpecialtyName IN VARCHAR2, p_Description IN VARCHAR2, o_SpecialtyId OUT NUMBER) IS
    BEGIN
        INSERT INTO Specialties (SpecialtyName, Description)
        VALUES (TRIM(p_SpecialtyName), p_Description)
        RETURNING SpecialtyId INTO o_SpecialtyId;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20035, 'Specialty name already exists.');
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20030, 'Error adding specialty: ' || SQLERRM);
    END ADD_SPECIALTY;

    -- تحديث اسم ووصف التخصص
    PROCEDURE UPDATE_SPECIALTY(p_SpecialtyId IN NUMBER, p_SpecialtyName IN VARCHAR2, p_Description IN VARCHAR2) IS
    BEGIN
        UPDATE Specialties
        SET SpecialtyName = TRIM(p_SpecialtyName), Description = p_Description
        WHERE SpecialtyId = p_SpecialtyId;

        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20031, 'Specialty not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20036, 'Specialty name already exists.');
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20032, 'Error updating specialty: ' || SQLERRM);
    END UPDATE_SPECIALTY;

    -- حذف تخصص من الجدول
    PROCEDURE DELETE_SPECIALTY(p_SpecialtyId IN NUMBER) IS
    BEGIN
        DELETE FROM Specialties WHERE SpecialtyId = p_SpecialtyId;
        IF SQL%ROWCOUNT = 0 THEN
            RAISE_APPLICATION_ERROR(-20033, 'Specialty not found.');
        END IF;
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            IF SQLCODE BETWEEN -20999 AND -20000 THEN RAISE; END IF;
            RAISE_APPLICATION_ERROR(-20034, 'Error deleting specialty: ' || SQLERRM);
    END DELETE_SPECIALTY;

    -- إرجاع كافة التخصصات مرتبة أبجدياً
    PROCEDURE GET_ALL_SPECIALTIES(o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT SpecialtyId, SpecialtyName, Description
            FROM Specialties
            ORDER BY SpecialtyName;
    END GET_ALL_SPECIALTIES;

    -- إرجاع تخصص واحد حسب ID
    PROCEDURE GET_SPECIALTY_BY_ID(p_SpecialtyId IN NUMBER, o_Cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN o_Cursor FOR
            SELECT SpecialtyId, SpecialtyName, Description
            FROM Specialties
            WHERE SpecialtyId = p_SpecialtyId;
    END GET_SPECIALTY_BY_ID;

END PKG_SPECIALTIES_MNG;
/

--------------------------------------------------------
--  Constraints for Table SPECIALTIES
--------------------------------------------------------

  ALTER TABLE "SPECIALTIES" MODIFY ("SPECIALTYID" NOT NULL ENABLE)
  ALTER TABLE "SPECIALTIES" MODIFY ("SPECIALTYNAME" NOT NULL ENABLE)
  ALTER TABLE "SPECIALTIES" ADD PRIMARY KEY ("SPECIALTYID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "SPECIALTIES" ADD CONSTRAINT "UQ_SPECIALTY_NAME" UNIQUE ("SPECIALTYNAME")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
--------------------------------------------------------
--  Constraints for Table DOCTORS
--------------------------------------------------------

  ALTER TABLE "DOCTORS" MODIFY ("DOCTORID" NOT NULL ENABLE)
  ALTER TABLE "DOCTORS" MODIFY ("FULLNAME" NOT NULL ENABLE)
  ALTER TABLE "DOCTORS" ADD PRIMARY KEY ("DOCTORID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "DOCTORS" ADD CONSTRAINT "CK_DOC_ACTIVE" CHECK (IsActive IN (0, 1)) ENABLE
  ALTER TABLE "DOCTORS" ADD CONSTRAINT "CK_DOC_FEE_POS" CHECK (ConsultationFee >= 0) ENABLE
--------------------------------------------------------
--  Constraints for Table ADMINS
--------------------------------------------------------

  ALTER TABLE "ADMINS" MODIFY ("ADMINID" NOT NULL ENABLE)
  ALTER TABLE "ADMINS" MODIFY ("ADMINNAME" NOT NULL ENABLE)
  ALTER TABLE "ADMINS" MODIFY ("PASSWORDHASH" NOT NULL ENABLE)
  ALTER TABLE "ADMINS" ADD PRIMARY KEY ("ADMINID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "ADMINS" ADD CONSTRAINT "CK_ADMIN_ACTIVE" CHECK (IsActive IN (0, 1)) ENABLE
  ALTER TABLE "ADMINS" ADD CONSTRAINT "UQ_ADMIN_NAME" UNIQUE ("ADMINNAME")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "ADMINS" ADD CONSTRAINT "UQ_ADMINS_EMAIL" UNIQUE ("EMAIL")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
--------------------------------------------------------
--  Constraints for Table APPOINTMENTS
--------------------------------------------------------

  ALTER TABLE "APPOINTMENTS" MODIFY ("APPOINTMENTID" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTS" MODIFY ("APPOINTMENTDATE" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTS" MODIFY ("APPOINTMENTTIME" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTS" ADD PRIMARY KEY ("APPOINTMENTID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "APPOINTMENTS" ADD UNIQUE ("REFERENCENUMBER")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "APPOINTMENTS" ADD CONSTRAINT "CK_APPT_STATUS" CHECK (Status IN ('Scheduled', 'Completed', 'Cancelled')) ENABLE
  ALTER TABLE "APPOINTMENTS" ADD CONSTRAINT "CK_APPT_TIME_FMT" CHECK (REGEXP_LIKE(AppointmentTime, '^((0?[1-9]|1[0-2]):[0-5][0-9] (AM|PM)|([01]?[0-9]|2[0-3]):[0-5][0-9])$')) ENABLE NOVALIDATE
--------------------------------------------------------
--  Constraints for Table ROLES
--------------------------------------------------------

  ALTER TABLE "ROLES" MODIFY ("ROLEID" NOT NULL ENABLE)
  ALTER TABLE "ROLES" MODIFY ("ROLENAME" NOT NULL ENABLE)
  ALTER TABLE "ROLES" ADD PRIMARY KEY ("ROLEID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "ROLES" ADD CONSTRAINT "UQ_ROLE_NAME" UNIQUE ("ROLENAME")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
--------------------------------------------------------
--  Constraints for Table APPOINTMENTSLOTS
--------------------------------------------------------

  ALTER TABLE "APPOINTMENTSLOTS" MODIFY ("SLOTID" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTSLOTS" MODIFY ("SLOTDATE" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTSLOTS" MODIFY ("SLOTTIME" NOT NULL ENABLE)
  ALTER TABLE "APPOINTMENTSLOTS" ADD PRIMARY KEY ("SLOTID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "APPOINTMENTSLOTS" ADD CONSTRAINT "UQ_DOCTOR_SLOT" UNIQUE ("DOCTORID", "SLOTDATE", "SLOTTIME")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "APPOINTMENTSLOTS" ADD CONSTRAINT "CK_SLOT_TIME_FMT" CHECK (REGEXP_LIKE(SlotTime, '^((0?[1-9]|1[0-2]):[0-5][0-9] (AM|PM)|([01]?[0-9]|2[0-3]):[0-5][0-9])$')) ENABLE NOVALIDATE
--------------------------------------------------------
--  Constraints for Table DOCTORWORKINGHOURS
--------------------------------------------------------

  ALTER TABLE "DOCTORWORKINGHOURS" MODIFY ("WORKINGHOURSID" NOT NULL ENABLE)
  ALTER TABLE "DOCTORWORKINGHOURS" MODIFY ("DAYOFWEEK" NOT NULL ENABLE)
  ALTER TABLE "DOCTORWORKINGHOURS" MODIFY ("STARTTIME" NOT NULL ENABLE)
  ALTER TABLE "DOCTORWORKINGHOURS" MODIFY ("ENDTIME" NOT NULL ENABLE)
  ALTER TABLE "DOCTORWORKINGHOURS" ADD PRIMARY KEY ("WORKINGHOURSID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "DOCTORWORKINGHOURS" ADD CONSTRAINT "CK_WORK_DAYS" CHECK (
    DayOfWeek IN ('السبت', 'الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت - الخميس', 
                  'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday')
) ENABLE
--------------------------------------------------------
--  Constraints for Table PATIENTS
--------------------------------------------------------

  ALTER TABLE "PATIENTS" MODIFY ("PATIENTID" NOT NULL ENABLE)
  ALTER TABLE "PATIENTS" MODIFY ("FULLNAME" NOT NULL ENABLE)
  ALTER TABLE "PATIENTS" MODIFY ("PHONENUMBER" NOT NULL ENABLE)
  ALTER TABLE "PATIENTS" MODIFY ("EMAIL" NOT NULL ENABLE)
  ALTER TABLE "PATIENTS" MODIFY ("DATEOFBIRTH" NOT NULL ENABLE)
  ALTER TABLE "PATIENTS" ADD PRIMARY KEY ("PATIENTID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "PATIENTS" ADD CONSTRAINT "CK_PAT_ACTIVE" CHECK (IsActive IN (0, 1)) ENABLE
  ALTER TABLE "PATIENTS" ADD CONSTRAINT "UQ_PATIENT_EMAIL" UNIQUE ("EMAIL")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
  ALTER TABLE "PATIENTS" ADD CONSTRAINT "UQ_PATIENT_PHONE" UNIQUE ("PHONENUMBER")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "USERS"  ENABLE
--------------------------------------------------------
--  Ref Constraints for Table ADMINS
--------------------------------------------------------

  ALTER TABLE "ADMINS" ADD CONSTRAINT "FK_ADMIN_ROLE" FOREIGN KEY ("ROLEID")
	  REFERENCES "ROLES" ("ROLEID") ENABLE
--------------------------------------------------------
--  Ref Constraints for Table APPOINTMENTS
--------------------------------------------------------

  ALTER TABLE "APPOINTMENTS" ADD CONSTRAINT "FK_APPOINTMENT_DOCTOR" FOREIGN KEY ("DOCTORID")
	  REFERENCES "DOCTORS" ("DOCTORID") ENABLE
  ALTER TABLE "APPOINTMENTS" ADD CONSTRAINT "FK_APPOINTMENT_SLOT" FOREIGN KEY ("SLOTID")
	  REFERENCES "APPOINTMENTSLOTS" ("SLOTID") ENABLE
  ALTER TABLE "APPOINTMENTS" ADD CONSTRAINT "FK_APPOINTMENT_PATIENT" FOREIGN KEY ("PATIENTID")
	  REFERENCES "PATIENTS" ("PATIENTID") ENABLE
--------------------------------------------------------
--  Ref Constraints for Table APPOINTMENTSLOTS
--------------------------------------------------------

  ALTER TABLE "APPOINTMENTSLOTS" ADD CONSTRAINT "FK_SLOT_DOCTOR" FOREIGN KEY ("DOCTORID")
	  REFERENCES "DOCTORS" ("DOCTORID") ON DELETE CASCADE ENABLE
--------------------------------------------------------
--  Ref Constraints for Table DOCTORS
--------------------------------------------------------

  ALTER TABLE "DOCTORS" ADD CONSTRAINT "FK_DOCTOR_SPECIALTY" FOREIGN KEY ("SPECIALTYID")
	  REFERENCES "SPECIALTIES" ("SPECIALTYID") ON DELETE SET NULL ENABLE
--------------------------------------------------------
--  Ref Constraints for Table DOCTORWORKINGHOURS
--------------------------------------------------------

  ALTER TABLE "DOCTORWORKINGHOURS" ADD CONSTRAINT "FK_WORKHOURS_DOCTOR" FOREIGN KEY ("DOCTORID")
	  REFERENCES "DOCTORS" ("DOCTORID") ON DELETE CASCADE ENABLE
