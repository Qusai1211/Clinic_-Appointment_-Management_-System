using Clinic_Managemeant.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Managemeant.Core.Repository
{
    public interface IAdminRepository
    {

        Task<long> AddAdminAsync(Admin admin);
        Task<(long? AdminId, bool IsSuccess, string? RoleName)> LoginAsync(string adminName, string passwordHash);
         Task<Admin> GetAdminByIdAsync(int adminId);
        Task<List<Admin>> GetAllAdminsAsync();
        
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> DeleteAdminAsync(int adminId);

    }
}
