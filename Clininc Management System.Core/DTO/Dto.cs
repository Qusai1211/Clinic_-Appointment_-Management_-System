using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.DTO
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public decimal? AdminId { get; set; }
        public string? AdminName { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class RegisterPatientRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class BookAppointmentRequest
    {
        public decimal DoctorId { get; set; }
        public decimal? PatientId { get; set; }
        public decimal SlotId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;

        // In case registering inline on booking page
        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? PatientEmail { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
    }

    public class BookAppointmentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public decimal? AppointmentId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? SpecialtyName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? Status { get; set; }
        public decimal? ConsultationFee { get; set; }
    }

    public class CancelAppointmentRequest
    {
        public decimal AppointmentId { get; set; }
    }

    public class UpdateAppointmentStatusRequest
    {
        public decimal AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DoctorUpsertDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Qualifications { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int IsFeatured { get; set; }
        public decimal? SpecialtyId { get; set; }
    }

    public class SpecialtyUpsertDto
    {
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class WorkingHoursUpsertDto
    {
        public decimal DoctorId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }

    public class SlotCreateDto
    {
        public decimal DoctorId { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; } = string.Empty;
    }

    public class PatientUpsertDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class SlotUpdateDto
    {
        public bool IsAvailable { get; set; }
    }
}
