using Clininc_Management_System.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Repository
{
    public interface IDoctorRepository
    {
        public  Task<List<Doctor>> GetAllDoctorsAsync();
        public  Task CreateDoctor(Doctor doctor);
        public  Task UpdateDoctor(Doctor doctor);
        public  Task DeleteDoctor(int id);
        public  Task<Doctor?> GetDoctorById(int id);

    }
}
