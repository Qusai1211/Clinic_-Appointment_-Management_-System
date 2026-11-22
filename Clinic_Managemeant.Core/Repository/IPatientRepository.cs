using Clinic_Managemeant.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Managemeant.Core.Repository
{
    public interface IPatientRepository
    {
        Task<long> AddPatientAsync(Patient patient);
       Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int patientId);
        Task<Patient> GetPatientByIdAsync(int patientId);
        Task<List<Patient>> GetAllPatientsAsync();
    }
}
