using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] DateTime? date, [FromQuery] string? status)
        {
            var appointments = await _appointmentService.GetAppointmentsAsync(search, date, status);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound(new { Message = $"Appointment with ID {id} not found." });
            }
            return Ok(appointment);
        }

        [HttpGet("reference/{referenceNumber}")]
        public async Task<IActionResult> GetByReference(string referenceNumber)
        {
            var appointment = await _appointmentService.GetAppointmentByReferenceAsync(referenceNumber);
            if (appointment == null)
            {
                return NotFound(new { Message = $"Appointment with reference number {referenceNumber} not found." });
            }
            return Ok(appointment);
        }

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookAppointmentRequest request)
        {
            if (request.DoctorId <= 0 || request.SlotId <= 0)
            {
                return BadRequest(new { Success = false, Message = "Valid DoctorId and SlotId are required." });
            }

            try
            {
                var result = await _appointmentService.BookAppointmentAsync(request);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Capture custom Oracle error messages (e.g., slot unavailable)
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("ORA-20002") || msg.Contains("no longer available"))
                {
                    return Conflict(new { Success = false, Message = "The selected appointment slot is no longer available. Please select another slot." });
                }
                if (msg.Contains("ORA-20007") || msg.Contains("does not match"))
                {
                    return BadRequest(new { Success = false, Message = "Selected slot does not match the requested doctor." });
                }

                return StatusCode(500, new { Success = false, Message = $"Booking error: {msg}" });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(decimal id)
        {
            try
            {
                var cancelled = await _appointmentService.CancelAppointmentAsync(id);
                return Ok(new { Success = true, Message = "Appointment cancelled successfully and slot reopened." });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("ORA-20008") || msg.Contains("already cancelled"))
                {
                    return BadRequest(new { Success = false, Message = "Appointment is already cancelled." });
                }
                return StatusCode(500, new { Success = false, Message = $"Cancellation failed: {msg}" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(decimal id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new { Success = false, Message = "Status is required." });
            }

            try
            {
                var updated = await _appointmentService.UpdateAppointmentStatusAsync(id, request.Status);
                return Ok(new { Success = true, Message = $"Appointment status updated to '{request.Status}' successfully." });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Success = false, Message = $"Failed to update status: {msg}" });
            }
        }
    }
}
