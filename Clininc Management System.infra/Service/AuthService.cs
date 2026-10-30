using Clininc_Management_System.Core.DTO;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.Core.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAdminRepository adminRepository, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponse { Success = false, Message = "Username and password are required." };
            }

            // جلب قائمة المشرفين والبحث عن المشرف برقم المستخدم (Adminname) مطابقةً لما هو موجود في الـ Repository
            var admins = await _adminRepository.GetAllAdmins();
            var admin = admins.FirstOrDefault(a => a.Adminname.ToLower() == request.Username.ToLower());

            if (admin == null)
            {
                return new LoginResponse { Success = false, Message = "Invalid administrator credentials." };
            }

            var isValid = Common.DbContext.VerifyPassword(request.Password, admin.Passwordhash, out bool needsRehash);
            if (!isValid)
            {
                return new LoginResponse { Success = false, Message = "Invalid administrator credentials." };
            }

            var secret = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrWhiteSpace(secret) || secret.Contains("YOUR_SECURE_JWT_SECRET"))
            {
                secret = "ClinicAppointmentManagementSystemDefaultDevKey2026!#";
            }
            var issuer = _configuration["JwtSettings:Issuer"] ?? "ClinicAPI";
            var audience = _configuration["JwtSettings:Audience"] ?? "ClinicClient";
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Adminid.ToString()),
                new Claim(ClaimTypes.Name, admin.Adminname),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new LoginResponse
            {
                Success = true,
                Message = "Authentication successful.",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                AdminId = admin.Adminid,
                AdminName = admin.Adminname,
                ExpiresAt = expiresAt
            };
        }
    }
}