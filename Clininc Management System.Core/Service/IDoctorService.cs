using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Service
{
    public interface IDoctorService
    {
        public Task<List<Doctor>> GetAllDoctorsAsync();
        public  Task<bool> CreateDoctor(Doctor doctor);
        public Task<bool> UpdateDoctor(Doctor doctor);
        public Task<bool> DeleteDoctor(int id);
        public Task<Doctor?> GetDoctorById(int id);


    }
}
