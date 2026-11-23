namespace Clininc_Management_System.Core.Models
{
    public class Admin
    {
        public decimal AdminId { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public decimal? RoleId { get; set; } = 1;
        public string? RoleName { get; set; } = "Admin";
        public int? IsActive { get; set; } = 1;
        public string? Email { get; set; }
    }

    public class Role
    {
        public decimal RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class AdminDashboardStats
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int ScheduledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
    }

    public class SpecialtyAppointmentChartItem
    {
        public decimal SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}
