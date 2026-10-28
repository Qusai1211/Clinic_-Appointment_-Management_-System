using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.Core.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Clininc_Management_System.infra.Service
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _repository;

        public SpecialtyService(ISpecialtyRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync() => _repository.GetAllAsync();
        public Task<Specialty?> GetSpecialtyByIdAsync(decimal specialtyId) => _repository.GetByIdAsync(specialtyId);
        public Task<decimal> CreateSpecialtyAsync(SpecialtyUpsertDto dto) => _repository.CreateAsync(dto);
        public Task<bool> UpdateSpecialtyAsync(decimal specialtyId, SpecialtyUpsertDto dto) => _repository.UpdateAsync(specialtyId, dto);
        public Task<bool> DeleteSpecialtyAsync(decimal specialtyId) => _repository.DeleteAsync(specialtyId);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;

        public DoctorService(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Doctor>> GetDoctorsAsync(string? search = null, decimal? specialtyId = null, string? sortBy = null)
            => _repository.GetAllAsync(search, specialtyId, sortBy);

        public Task<IEnumerable<Doctor>> GetFeaturedDoctorsAsync()
            => _repository.GetFeaturedAsync();

        public Task<Doctor?> GetDoctorDetailsAsync(decimal doctorId)
            => _repository.GetByIdAsync(doctorId);

        public Task<decimal> CreateDoctorAsync(DoctorUpsertDto dto)
            => _repository.CreateAsync(dto);

        public Task<bool> UpdateDoctorAsync(decimal doctorId, DoctorUpsertDto dto)
            => _repository.UpdateAsync(doctorId, dto);

        public Task<bool> DeleteDoctorAsync(decimal doctorId)
            => _repository.DeleteAsync(doctorId);

        public Task<IEnumerable<DoctorWorkingHours>> GetWorkingHoursAsync(decimal doctorId)
            => _repository.GetWorkingHoursByDoctorAsync(doctorId);

        public Task<decimal> AddWorkingHoursAsync(WorkingHoursUpsertDto dto)
            => _repository.AddWorkingHoursAsync(dto);

        public Task<bool> DeleteWorkingHoursAsync(decimal workingHoursId)
            => _repository.DeleteWorkingHoursAsync(workingHoursId);

        public Task<bool> UpdateWorkingHoursAsync(decimal workingHoursId, WorkingHoursUpsertDto dto)
            => _repository.UpdateWorkingHoursAsync(workingHoursId, dto);

        public Task<IEnumerable<AppointmentSlot>> GetDoctorSlotsAsync(decimal doctorId, DateTime? date = null, bool availableOnly = false)
            => _repository.GetSlotsByDoctorAsync(doctorId, date, availableOnly);

        public Task<decimal> AddSlotAsync(SlotCreateDto dto)
            => _repository.AddSlotAsync(dto);

        public Task<bool> UpdateSlotStatusAsync(decimal slotId, bool isAvailable)
            => _repository.UpdateSlotStatusAsync(slotId, isAvailable);

        public Task<bool> DeleteSlotAsync(decimal slotId)
            => _repository.DeleteSlotAsync(slotId);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null)
            => _repository.GetAllAsync(search);

        public Task<Patient?> GetPatientByIdAsync(decimal patientId)
            => _repository.GetByIdAsync(patientId);

        public Task<decimal> RegisterPatientAsync(RegisterPatientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("Phone number is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");

            return _repository.RegisterPatientAsync(request);
        }

        public Task<bool> UpdatePatientAsync(decimal patientId, PatientUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("Phone number is required.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            return _repository.UpdateAsync(patientId, dto);
        }

        public Task<bool> DeletePatientAsync(decimal patientId)
            => _repository.DeleteAsync(patientId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public Task<IEnumerable<Appointment>> GetAppointmentsAsync(string? search = null, DateTime? date = null, string? status = null)
            => _appointmentRepository.GetAllAsync(search, date, status);

        public Task<Appointment?> GetAppointmentByIdAsync(decimal appointmentId)
            => _appointmentRepository.GetByIdAsync(appointmentId);

        public Task<Appointment?> GetAppointmentByReferenceAsync(string referenceNumber)
            => _appointmentRepository.GetByReferenceNumberAsync(referenceNumber);

        public async Task<BookAppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request)
        {
            // If patientId is not provided, register/retrieve patient first
            if (!request.PatientId.HasValue || request.PatientId.Value <= 0)
            {
                if (string.IsNullOrWhiteSpace(request.PatientName) ||
                    string.IsNullOrWhiteSpace(request.PatientPhone) ||
                    string.IsNullOrWhiteSpace(request.PatientEmail))
                {
                    return new BookAppointmentResponse
                    {
                        Success = false,
                        Message = "Patient details (name, phone, email) are required for booking."
                    };
                }

                var patientId = await _patientRepository.RegisterPatientAsync(new RegisterPatientRequest
                {
                    FullName = request.PatientName.Trim(),
                    PhoneNumber = request.PatientPhone.Trim(),
                    Email = request.PatientEmail.Trim(),
                    DateOfBirth = request.PatientDateOfBirth ?? DateTime.UtcNow.AddYears(-25)
                });

                request.PatientId = patientId;
            }

            var (appId, refNum) = await _appointmentRepository.BookAppointmentAsync(request);

            var createdApp = await _appointmentRepository.GetByIdAsync(appId);

            return new BookAppointmentResponse
            {
                Success = true,
                Message = "Appointment booked successfully.",
                AppointmentId = appId,
                ReferenceNumber = refNum,
                PatientName = createdApp?.PatientName ?? request.PatientName,
                DoctorName = createdApp?.DoctorName,
                SpecialtyName = createdApp?.SpecialtyName,
                AppointmentDate = createdApp?.AppointmentDate ?? request.AppointmentDate,
                AppointmentTime = createdApp?.AppointmentTime ?? request.AppointmentTime,
                Status = createdApp?.Status ?? "Scheduled",
                ConsultationFee = createdApp?.ConsultationFee
            };
        }

        public Task<bool> CancelAppointmentAsync(decimal appointmentId)
            => _appointmentRepository.CancelAppointmentAsync(appointmentId);

        public Task<bool> UpdateAppointmentStatusAsync(decimal appointmentId, string status)
            => _appointmentRepository.UpdateStatusAsync(appointmentId, status);
    }

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

            var admin = await _adminRepository.GetByUsernameAsync(request.Username);
            if (admin == null)
            {
                return new LoginResponse { Success = false, Message = "Invalid administrator credentials." };
            }

            var isValid = Common.PasswordSecurity.VerifyPassword(request.Password, admin.PasswordHash, out bool needsRehash);
            if (!isValid)
            {
                return new LoginResponse { Success = false, Message = "Invalid administrator credentials." };
            }

            if (needsRehash)
            {
                // Transparently upgrade legacy hash to standard PBKDF2 with 100k iterations and secure salt
                var upgradedHash = Common.PasswordSecurity.HashPassword(request.Password);
                await _adminRepository.UpdatePasswordHashAsync(admin.AdminId, upgradedHash);
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
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Name, admin.AdminName),
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
                AdminId = admin.AdminId,
                AdminName = admin.AdminName,
                ExpiresAt = expiresAt
            };
        }
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<AdminDashboardStats> GetDashboardStatsAsync() => _repository.GetStatsAsync();
        public Task<IEnumerable<SpecialtyAppointmentChartItem>> GetSpecialtyChartDataAsync() => _repository.GetAppointmentsBySpecialtyAsync();
    }
}

