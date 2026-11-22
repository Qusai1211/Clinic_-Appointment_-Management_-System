using Clinic_Managemeant.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Managemeant.Core.Service
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<long> AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int appointmentId);
        Task<Appointment> GetAppointmentByIdAsync(int appointmentId);
    }
}