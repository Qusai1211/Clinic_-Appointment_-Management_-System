using Clininc_Management_System.Core.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Clininc_Management_System.infra.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _localSinkPath;
        private readonly string _deliveryMode;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _deliveryMode = _configuration["EmailSettings:SafeDeliveryMode"] ?? "LocalSink";
            _localSinkPath = _configuration["EmailSettings:LocalSinkPath"] ?? Path.Combine(AppContext.BaseDirectory, "MailSink");
            _senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "no-reply@clinic.com";
            _senderName = _configuration["EmailSettings:SenderName"] ?? "Clinic Appointment Management System";

            if (_deliveryMode.Equals("LocalSink", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    if (!Directory.Exists(_localSinkPath))
                    {
                        Directory.CreateDirectory(_localSinkPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not create email sink directory at {Path}", _localSinkPath);
                }
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("SendEmailAsync called with null or empty recipient.");
                return false;
            }

            try
            {
                var emailRecord = new
                {
                    Timestamp = DateTime.UtcNow,
                    To = toEmail,
                    From = $"{_senderName} <{_senderEmail}>",
                    Subject = subject,
                    IsHtml = isHtml,
                    Body = body,
                    DeliveryMode = _deliveryMode
                };

                // Safe Local Sink Delivery (default mode to prevent accidental spam / unverified SMTP failures)
                if (_deliveryMode.Equals("LocalSink", StringComparison.OrdinalIgnoreCase))
                {
                    var fileName = $"Email_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString()[..8]}.json";
                    var fullPath = Path.Combine(_localSinkPath, fileName);
                    var json = JsonSerializer.Serialize(emailRecord, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(fullPath, json);

                    _logger.LogInformation("[Email LocalSink] Email saved to {Path}. Subject: '{Subject}', To: {To}", fullPath, subject, toEmail);
                    return true;
                }

                // SMTP Delivery
                if (_deliveryMode.Equals("Smtp", StringComparison.OrdinalIgnoreCase))
                {
                    var smtpHost = _configuration["EmailSettings:SmtpServer"] ?? "localhost";
                    var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var p) ? p : 25;
                    var enableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out var ssl) && ssl;
                    var username = _configuration["EmailSettings:Username"];
                    var password = _configuration["EmailSettings:Password"];

                    using var message = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, _senderName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };
                    message.To.Add(toEmail);

                    using var client = new SmtpClient(smtpHost, smtpPort)
                    {
                        EnableSsl = enableSsl
                    };

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        client.Credentials = new NetworkCredential(username, password);
                    }

                    await client.SendMailAsync(message);
                    _logger.LogInformation("[Email SMTP] Successfully delivered email to {To}. Subject: '{Subject}'", toEmail, subject);
                    return true;
                }

                _logger.LogInformation("[Email Disabled] Delivery skipped (DeliveryMode={Mode}) for {To}", _deliveryMode, toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Subject: '{Subject}'", toEmail, subject);
                return false;
            }
        }

        public async Task<bool> SendAppointmentConfirmationAsync(string patientEmail, string patientName, string doctorName, string appointmentDate, string appointmentTime, string referenceNumber)
        {
            var subject = $"Appointment Confirmation - Ref #{referenceNumber}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #2563eb;'>Clinic Appointment Confirmation</h2>
                    <p>Dear <strong>{patientName}</strong>,</p>
                    <p>Your appointment has been successfully scheduled. Here are the confirmation details:</p>
                    <table style='width: 100%; border-collapse: collapse; margin: 16px 0;'>
                        <tr><td style='padding: 8px; border-bottom: 1px solid #eee; color: #666;'>Reference Number:</td><td style='padding: 8px; border-bottom: 1px solid #eee; font-weight: bold; color: #1e40af;'>{referenceNumber}</td></tr>
                        <tr><td style='padding: 8px; border-bottom: 1px solid #eee; color: #666;'>Doctor:</td><td style='padding: 8px; border-bottom: 1px solid #eee; font-weight: bold;'>{doctorName}</td></tr>
                        <tr><td style='padding: 8px; border-bottom: 1px solid #eee; color: #666;'>Date:</td><td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointmentDate}</td></tr>
                        <tr><td style='padding: 8px; border-bottom: 1px solid #eee; color: #666;'>Time:</td><td style='padding: 8px; border-bottom: 1px solid #eee;'>{appointmentTime}</td></tr>
                        <tr><td style='padding: 8px; border-bottom: 1px solid #eee; color: #666;'>Status:</td><td style='padding: 8px; border-bottom: 1px solid #eee; color: #16a34a; font-weight: bold;'>Scheduled</td></tr>
                    </table>
                    <p style='color: #555; font-size: 14px;'>Please present your reference number upon arrival at the clinic reception desk.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'/>
                    <p style='font-size: 12px; color: #888;'>This is an automated notification from Clinic Appointment Management System.</p>
                </div>";

            return await SendEmailAsync(patientEmail, subject, body, isHtml: true);
        }

        public async Task<bool> SendAppointmentCancellationAsync(string patientEmail, string patientName, string doctorName, string appointmentDate, string appointmentTime, string referenceNumber)
        {
            var subject = $"Appointment Cancellation - Ref #{referenceNumber}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #dc2626;'>Appointment Cancelled</h2>
                    <p>Dear <strong>{patientName}</strong>,</p>
                    <p>Your appointment with reference number <strong>{referenceNumber}</strong> has been cancelled.</p>
                    <p><strong>Doctor:</strong> {doctorName}<br/><strong>Scheduled Date:</strong> {appointmentDate}<br/><strong>Scheduled Time:</strong> {appointmentTime}</p>
                    <p>If you wish to reschedule, please visit our clinic portal.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'/>
                    <p style='font-size: 12px; color: #888;'>Clinic Appointment Management System</p>
                </div>";

            return await SendEmailAsync(patientEmail, subject, body, isHtml: true);
        }

        public async Task<bool> SendAdminNotificationAsync(string adminEmail, string subject, string message)
        {
            var body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h3>Admin Alert: {subject}</h3>
                    <p>{message}</p>
                    <p style='color: #888; font-size: 12px;'>Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                </div>";

            return await SendEmailAsync(adminEmail, subject, body, isHtml: true);
        }
    }
}
