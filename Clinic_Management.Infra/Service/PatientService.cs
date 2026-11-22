using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Clinic_Managemeant.Core.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Service
{
    public class PatientService: IPatientService
    {
        private readonly IPatientRepository _repository;
        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> AddPatientAsync(Patient patient)
        {
            return await _repository.AddPatientAsync(patient);
        }
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
           
             await _repository.UpdatePatientAsync(patient);
            return true;
        }
        public async Task<bool> DeletePatientAsync(int patientId)
        {
            return await _repository.DeletePatientAsync(patientId);
        }
        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            return await _repository.GetPatientByIdAsync(patientId);
        }
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            
           var pationts= await _repository.GetAllPatientsAsync();
            return pationts; 
        }
    }
}

