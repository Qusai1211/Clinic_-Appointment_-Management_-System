using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Clinic_Mangemant.API.Controllers;
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
            var adminId = await _adminService.AddAdminAsync(admin);
            return Ok(new { AdminId = adminId });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء إضافة المشرف.", Details = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            var (adminId, isSuccess, roleName) = await _adminService.LoginAsync(
                request.AdminName,
                request.PasswordHash);

            if (!isSuccess || adminId is null)
            {
                return Unauthorized(new { Message = "اسم المستخدم أو كلمة المرور غير صحيحة." });
            }

            var resolvedRole = string.IsNullOrWhiteSpace(roleName) ? "Admin" : roleName;
            var token = GenerateJwtToken(adminId.Value, request.AdminName, resolvedRole);

            return Ok(new
            {
                Token = token,
                AdminId = adminId.Value,
                RoleName = resolvedRole
            });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء تسجيل الدخول.", Details = ex.Message });
        }
    }
    [HttpPut]
    [Route("update/{adminId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAdmin( [FromBody] Admin admin)
    {
        try
        {
            var isSuccess = await _adminService.UpdateAdminAsync( admin);
            if (isSuccess)
            {
                return Ok(new { Message = "تم تحديث بيانات المشرف بنجاح." });
            }
            return NotFound(new { Message = "المشرف غير موجود." });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء تحديث بيانات المشرف.", Details = ex.Message });
        }
    }


    [HttpDelete]
    [Route("delete/{adminId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAdmin(int adminId)
    {
        try
        {
            var isSuccess = await _adminService.DeleteAdminAsync(adminId);
            if (isSuccess)
            {
                return Ok(new { Message = "تم حذف المشرف بنجاح." });
            }

            return NotFound(new { Message = "المشرف غير موجود." });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء حذف المشرف.", Details = ex.Message });
        }
    }

    [HttpGet]
    [Route("GetAllAdmins")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdminsAsync()
    {
        try
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return Ok(admins);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء جلب بيانات المشرفين.", Details = ex.Message });
        }
    }

    [HttpGet("GetById/{adminId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Admin>> GetAdminByIdAsync(int adminId)
    {
        try
        {
            var admin = await _adminService.GetAdminByIdAsync(adminId);
            if (admin is null)
            {
                return NotFound(new { Message = "المشرف غير موجود" });
            }

            return Ok(admin);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { Message = "حدث خطأ أثناء جلب بيانات المشرف.", Details = ex.Message });
        }
    }

    private string GenerateJwtToken(long adminId, string adminName, string roleName)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? "MyTrainingJWTSecurityKey2026Secure!";
        var issuer = jwtSection["Issuer"] ?? "ClinicManagementApi";
        var audience = jwtSection["Audience"] ?? "ClinicManagementClient";

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

public sealed record LoginRequest(string AdminName, string PasswordHash);