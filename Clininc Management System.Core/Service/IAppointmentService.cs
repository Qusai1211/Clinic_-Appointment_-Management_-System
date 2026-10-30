using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Service
{
    public interface IAppointmentService
    {
        public Task<List<Appointment>> GetAppointments();
        public Task CreateAppointment(Appointment appointment);
        public Task UpdateAppointment(Appointment appointment);
        public Task DeleteAppointment(int id);
        public Task<Appointment?> GetAppointment(int id);

    }
}
