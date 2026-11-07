using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Service;
using Clininc_Management_System.infra.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService appointmentService;
        public AppointmentController(IAppointmentService _appointmentService)
        {
            appointmentService = _appointmentService;
        }
        [HttpGet]
        public Task<List<Appointment>> GetAppointments()
        {
            return appointmentService.GetAppointments();
        }
        [HttpPost]
        public Task CreateAppointment(Appointment appointment)
        {
            return appointmentService.CreateAppointment(appointment);
        }
         
        [HttpPut]

        public Task UpdateAppointment(Appointment appointment)
        {
            return appointmentService.UpdateAppointment(appointment);
        }
        [HttpDelete]
        [Route("delet/{id}")]

        public Task DeleteAppointment(int id)
        {

            return appointmentService.DeleteAppointment(id);    
        }
        [HttpGet]
        [Route("getaappointment{id}")]

        public Task<Appointment?> GetAppointment(int id)
        {
              
            return appointmentService.GetAppointment(id);   
        }

    }
}
