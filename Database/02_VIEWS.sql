-- ============================================================================
-- Clinic Appointment Management System
-- Tier 1: Database - 02_VIEWS.sql
-- Compatible with Oracle Database 19c / 21c / XE
-- ============================================================================

SET DEFINE OFF;

-- ----------------------------------------------------------------------------
-- 1. View: Vw_AdminDashboardStats
-- Live operational metrics for administrator dashboard
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW Vw_AdminDashboardStats AS
SELECT 
    (SELECT COUNT(*) FROM Doctors) AS TotalDoctors,
    (SELECT COUNT(*) FROM Patients) AS TotalPatients,
    (SELECT COUNT(*) FROM Appointments) AS TotalAppointments,
    (SELECT COUNT(*) FROM Appointments WHERE Status IN ('Scheduled', 'Confirmed')) AS ScheduledAppointments,
    (SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed') AS CompletedAppointments,
    (SELECT COUNT(*) FROM Appointments WHERE Status = 'Cancelled') AS CancelledAppointments
FROM DUAL;

-- ----------------------------------------------------------------------------
-- 2. View: Vw_AppointmentsBySpecialty
-- Analytical distribution of appointments across medical specialties
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW Vw_AppointmentsBySpecialty AS
SELECT 
    s.SpecialtyId,
    s.SpecialtyName,
    COUNT(a.AppointmentId) AS AppointmentCount
FROM Specialties s
LEFT JOIN Doctors d ON s.SpecialtyId = d.SpecialtyId
LEFT JOIN Appointments a ON d.DoctorId = a.DoctorId
GROUP BY s.SpecialtyId, s.SpecialtyName;
