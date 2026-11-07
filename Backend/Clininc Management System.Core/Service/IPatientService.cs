using Clininc_Management_System.Core.DTO;
using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Service
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null);
        Task<Patient?> GetPatientByIdAsync(decimal patientId);
        Task<decimal> RegisterPatientAsync(RegisterPatientRequest request);
        Task<bool> UpdatePatientAsync(decimal patientId, PatientUpsertDto dto);
        Task<bool> DeletePatientAsync(decimal patientId);
    }
}
