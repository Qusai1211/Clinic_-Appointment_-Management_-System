using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Clinic_Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet]
        [Route("GetId/{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "الموعد غير موجود." });

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
                return BadRequest(new { message = "بيانات الموعد غير صالحة." });

            var newAppointmentId = await _appointmentService.AddAppointmentAsync(appointment);
            return Ok(new { message = "تم حجز الموعد بنجاح", appointmentId = newAppointmentId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
                return BadRequest(new { message = "بيانات التحديث غير صالحة." });

            await _appointmentService.UpdateAppointmentAsync(appointment);
            return Ok(new { message = "تم تحديث تفاصيل الموعد بنجاح" });
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            await _appointmentService.DeleteAppointmentAsync(id);
            return Ok(new { message = "تم إلغاء الموعد بنجاح" });
        }
    }
}