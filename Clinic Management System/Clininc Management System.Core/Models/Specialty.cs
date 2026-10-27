namespace Clininc_Management_System.Core.Models
{
    public class Specialty
    {
        public decimal SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DoctorCount { get; set; }
        public int AppointmentCount { get; set; }
    }
}
