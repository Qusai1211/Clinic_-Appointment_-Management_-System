using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Clinic_Managemeant.Core.Service;

namespace Clinic_Management.Infra.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public async Task<long> AddAdminAsync(Admin admin)
        {
            // يمكنك إضافة أي منطق إضافي هنا قبل استدعاء المستودع
            return await _adminRepository.AddAdminAsync(admin);
        }
        public async Task<(long? AdminId, bool IsSuccess, string? RoleName)> LoginAsync(string adminName, string passwordHash)
        {
            return await _adminRepository.LoginAsync(adminName, passwordHash);
        }
    public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            return await _adminRepository.UpdateAdminAsync(admin);
        }
        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            return await _adminRepository.DeleteAdminAsync(adminId);
        }
        public async Task<Admin> GetAdminByIdAsync(int adminId)
        {
            return await _adminRepository.GetAdminByIdAsync(adminId);
        }
        public async Task<List<Admin>> GetAllAdminsAsync()
        {
            return await _adminRepository.GetAllAdminsAsync();
        }
    }
}
