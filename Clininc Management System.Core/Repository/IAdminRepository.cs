using Clininc_Management_System.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.Repository
{
    public interface IAdminRepository
    {
      public  Task<List<Admin>> GetAllAdmins();
      public  Task creatAdmin(Admin admin);
      public  Task updateAdmin(Admin admin);
       public Task deleteAdmin(int id);
       public Task<Admin?> GetAdminId(int id);
    }
}