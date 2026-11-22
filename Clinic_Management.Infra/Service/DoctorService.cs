using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Clinic_Managemeant.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        public DoctorService(IDoctorRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
           var doctors = await _repository.GetAllDoctorsAsync();
           return doctors;
        }
        public async Task<long> AddDoctorAsync(Doctor doctor)
        {
            return await _repository.AddDoctorAsync(doctor);
        }
       public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            return await _repository.UpdateDoctorAsync(doctor);
        }
        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            return await _repository.DeleteDoctorAsync(doctorId);
        }
        public async Task<Doctor> GetDoctorAsync(int doctorId)
        {
            return await _repository.GetDoctorAsync(doctorId);
        }
       
    }
}
