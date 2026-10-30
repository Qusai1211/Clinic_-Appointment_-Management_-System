using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Clininc_Management_System.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdmins();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminService.GetAdminId(id);
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
        {
            await _adminService.creatAdmin(admin);
            return Ok(new { message = "Admin created successfully with encrypted password!" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdmin([FromBody] Admin admin)
        {
            await _adminService.updateAdmin(admin);
            return Ok(new { message = "Admin updated successfully!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            await _adminService.deleteAdmin(id);
            return Ok(new { message = "Admin deleted successfully!" });
        }
    }
}