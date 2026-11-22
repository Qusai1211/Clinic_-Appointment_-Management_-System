using Clinic_Managemeant.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Managemeant.Core.Repository
{
    public interface IDoctorRepository
    {
        Task<long> AddDoctorAsync(Doctor doctor);
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<bool> UpdateDoctorAsync(Doctor doctor);
        Task<bool> DeleteDoctorAsync(int doctorId);
        Task<Doctor> GetDoctorAsync(int doctorId);


    }
}
