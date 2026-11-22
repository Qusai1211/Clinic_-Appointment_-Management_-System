using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Clinic_Managemeant.Core.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Service
{

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAppointmentsAsync();
        }

        public async Task<long> AddAppointmentAsync(Appointment appointment)
        {
            // يمكنك هنا إضافة أي منطق أعمال (Business Logic) قبل الحفظ، مثل التحقق من صحة الموعد
            return await _appointmentRepository.AddAppointmentAsync(appointment);
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {

            await _appointmentRepository.DeleteAppointmentAsync(appointmentId);


        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }

        
    }
}