using Clininc_Management_System.Core.DTO;
using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Repository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync(string? search = null);
        Task<Patient?> GetByIdAsync(decimal patientId);
        Task<decimal> RegisterPatientAsync(RegisterPatientRequest request);
        Task<bool> UpdateAsync(decimal patientId, PatientUpsertDto dto);
        Task<bool> DeleteAsync(decimal patientId);
    }
}
