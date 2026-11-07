using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.Core.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Service
{
    public class AdminService: IAdminService
    {
        private readonly IAdminRepository adminRepository;

        public AdminService(IAdminRepository _adminRepository)
        {
            adminRepository = _adminRepository;
        }

        // جلب كل المشرفين (Async)
        public async Task<List<Admin>> GetAllAdmins()
        {
            return await adminRepository.GetAllAdmins();
        }

        // إضافة مشرف جديد (Async)
        public async Task creatAdmin(Admin admin)
        {
            await adminRepository.creatAdmin(admin);
        }

        // تعديل مشرف (Async)
        public async Task updateAdmin(Admin admin)
        {
            await adminRepository.updateAdmin(admin);
        }

        // حذف مشرف (Async)
        public async Task deleteAdmin(int id)
        {
            await adminRepository.deleteAdmin(id);
        }

        // جلب مشرف برقم المعرف (Async)
        public async Task<Admin?> GetAdminId(int id)
        {

            return await adminRepository.GetAdminId(id);
        }
       
    }
}