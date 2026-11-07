using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Service
{
    public class AppointmentService: IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentService(IAppointmentRepository _appointmentRepository)
        {
            appointmentRepository = _appointmentRepository;
        }

        // جلب كل المواعيد (Async)
        public async Task<List<Appointment>> GetAppointments()
        {
            return await appointmentRepository.GetAppointments();
        }

        // إضافة موعد جديد (Async)
        public async Task CreateAppointment(Appointment appointment)
        {
            await appointmentRepository.createAppointment(appointment);
        }

        // تعديل موعد (Async)
        public async Task UpdateAppointment(Appointment appointment)
        {
            await appointmentRepository.updateAppointment(appointment);
        }

        // حذف موعد (Async)
        public async Task DeleteAppointment(int id)
        {
            await appointmentRepository.deleteAppointment(id);
        }

        // جلب موعد برقم المعرف (Async)
        public async Task<Appointment?> GetAppointment(int id)
        {
            return await appointmentRepository.GetAppointment(id);
        }
    }
}