namespace Clininc_Management_System.Core.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task<bool> SendAppointmentConfirmationAsync(string patientEmail, string patientName, string doctorName, string appointmentDate, string appointmentTime, string referenceNumber);
        Task<bool> SendAppointmentCancellationAsync(string patientEmail, string patientName, string doctorName, string appointmentDate, string appointmentTime, string referenceNumber);
        Task<bool> SendAdminNotificationAsync(string adminEmail, string subject, string message);
    }
}
