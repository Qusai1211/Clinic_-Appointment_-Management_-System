namespace Clininc_Management_System.Core.Models
{
    public class Doctor
    {
        public decimal DoctorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Qualifications { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? ProfileImageUrl { get; set; }
        public int IsFeatured { get; set; }
        public decimal? SpecialtyId { get; set; }
        public string? SpecialtyName { get; set; }

        public List<DoctorWorkingHours> WorkingHours { get; set; } = new();
        public List<AppointmentSlot> AvailableSlots { get; set; } = new();
    }

    public class DoctorWorkingHours
    {
        public decimal WorkingHoursId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public decimal DoctorId { get; set; }
    }

    public class AppointmentSlot
    {
        public decimal SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string SlotTime { get; set; } = string.Empty;
        public int IsAvailable { get; set; }
        public decimal DoctorId { get; set; }
        public string? DoctorName { get; set; }
    }
}
