using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Clinic_Management_System.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;

        public AdminController(IAdminService adminService, IConfiguration configuration)
        {
            _adminService = adminService;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] Admin admin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(admin.AdminName) || string.IsNullOrWhiteSpace(admin.PasswordHash))
                {
                    return BadRequest(new { Message = "Admin username and password are required." });
                }

                var adminId = await _adminService.AddAdminAsync(admin);
                return Ok(new { AdminId = adminId, Message = "Admin added successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while adding admin.", Details = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] AdminLoginRequest request)
        {
            try
            {
                var username = request.AdminName ?? request.Username;
                var password = request.PasswordHash ?? request.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return BadRequest(new { Message = "Username and password are required." });
                }

                var (adminId, isSuccess, roleName) = await _adminService.LoginAsync(username, password);

                if (!isSuccess || adminId is null)
                {
                    return Unauthorized(new { Message = "Invalid administrator credentials." });
                }

                var resolvedRole = string.IsNullOrWhiteSpace(roleName) ? "Admin" : roleName;
                var token = GenerateJwtToken(adminId.Value, username, resolvedRole);

                return Ok(new
                {
                    Success = true,
                    Token = token,
                    AdminId = adminId.Value,
                    AdminName = username,
                    RoleName = resolvedRole
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred during login.", Details = ex.Message });
            }
        }

        [HttpPut("update/{adminId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAdmin(int adminId, [FromBody] Admin admin)
        {
            try
            {
                admin.AdminId = adminId;
                var isSuccess = await _adminService.UpdateAdminAsync(admin);
                if (isSuccess)
                {
                    return Ok(new { Message = "Admin updated successfully." });
                }
                return NotFound(new { Message = "Admin not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while updating admin.", Details = ex.Message });
            }
        }

        [HttpDelete("delete/{adminId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdmin(int adminId)
        {
            try
            {
                var isSuccess = await _adminService.DeleteAdminAsync(adminId);
                if (isSuccess)
                {
                    return Ok(new { Message = "Admin deleted successfully." });
                }
                return NotFound(new { Message = "Admin not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while deleting admin.", Details = ex.Message });
            }
        }

        [HttpDelete("by-name/{adminName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdminByName(string adminName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adminName))
                {
                    return BadRequest(new { Message = "Administrator name is required." });
                }

                // Safeguard 1: Prevent self-deletion
                var currentAdmin = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrWhiteSpace(currentAdmin) && currentAdmin.Equals(adminName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { Message = "Self-deletion is prohibited. An administrator cannot delete their own account." });
                }

                // Safeguard 2: Prevent last-active-admin lockout
                var activeCount = await _adminService.GetActiveAdminCountAsync();
                if (activeCount <= 1)
                {
                    return BadRequest(new { Message = "Operation blocked: Cannot delete the last remaining active administrator." });
                }

                var isSuccess = await _adminService.DeleteAdminByNameAsync(adminName.Trim());
                if (isSuccess)
                {
                    return Ok(new { Message = $"Administrator '{adminName.Trim()}' deleted successfully." });
                }

                return NotFound(new { Message = $"Administrator '{adminName}' not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while deleting admin by name.", Details = ex.Message });
            }
        }

        [HttpGet("GetAllAdmins")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdminsAsync()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                var response = admins.Select(a => new Clininc_Management_System.Core.DTOs.AdminResponseDto
                {
                    AdminId = a.AdminId,
                    AdminName = a.AdminName,
                    RoleId = a.RoleId,
                    RoleName = a.RoleName,
                    Email = a.Email,
                    IsActive = a.IsActive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while retrieving admins.", Details = ex.Message });
            }
        }

        [HttpGet("GetById/{adminId:int}")]
        [HttpGet("{adminId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Clininc_Management_System.Core.DTOs.AdminResponseDto>> GetAdminByIdAsync(int adminId)
        {
            try
            {
                var admin = await _adminService.GetAdminDetailsByIdAsync(adminId);
                if (admin is null)
                {
                    return NotFound(new { Message = "Admin not found." });
                }

                return Ok(admin);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { Message = "Error occurred while retrieving admin.", Details = ex.Message });
            }
        }

        private string GenerateJwtToken(decimal adminId, string adminName, string roleName)
        {
            var jwtSection = _configuration.GetSection("JwtSettings");
            var key = jwtSection["Secret"] ?? _configuration["Jwt:Key"] ?? "TemporaryFallbackKeyForDesignTimeBuildOnly_abcdefghijklmnopqrstuvwxyz!";
            var issuer = jwtSection["Issuer"] ?? _configuration["Jwt:Issuer"] ?? "ClinicAPI";
            var audience = jwtSection["Audience"] ?? _configuration["Jwt:Audience"] ?? "ClinicClient";

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, adminId.ToString()),
                new(ClaimTypes.NameIdentifier, adminId.ToString()),
                new(ClaimTypes.Name, adminName),
                new(ClaimTypes.Role, roleName)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public sealed record AdminLoginRequest(string? AdminName, string? PasswordHash, string? Username, string? Password);
}
