using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Repository
{
    public interface IAppointmentRepository
    {
       public Task<List<Appointment>> GetAppointments();
        public Task createAppointment(Appointment appointment);
        public Task updateAppointment(Appointment appointment);
        public  Task deleteAppointment(int id);
        public  Task<Appointment?> GetAppointment(int id);


    }
}
