using Clininc_Management_System.Core.DTO;
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
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null)
            => _repository.GetAllAsync(search);

        public Task<Patient?> GetPatientByIdAsync(decimal patientId)
            => _repository.GetByIdAsync(patientId);

        public Task<decimal> RegisterPatientAsync(RegisterPatientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("Phone number is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            return _repository.RegisterPatientAsync(request);
        }

        public Task<bool> UpdatePatientAsync(decimal patientId, PatientUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            return _repository.UpdateAsync(patientId, dto);
        }

        public Task<bool> DeletePatientAsync(decimal patientId)
            => _repository.DeleteAsync(patientId);
    }
}

    
