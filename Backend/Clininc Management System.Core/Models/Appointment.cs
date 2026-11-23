namespace Clininc_Management_System.Core.Models
{
    public class Appointment
    {
        public decimal AppointmentId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public decimal? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string? SpecialtyName { get; set; }
        public decimal? SlotId { get; set; }
        public decimal? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? PatientEmail { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}

