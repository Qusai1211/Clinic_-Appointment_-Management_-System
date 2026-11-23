using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Models;

namespace Clininc_Management_System.Core.Service
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync();
        Task<Specialty?> GetSpecialtyByIdAsync(decimal specialtyId);
        Task<decimal> CreateSpecialtyAsync(SpecialtyUpsertDto dto);
        Task<bool> UpdateSpecialtyAsync(decimal specialtyId, SpecialtyUpsertDto dto);
        Task<bool> DeleteSpecialtyAsync(decimal specialtyId);
    }

    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetDoctorsAsync(string? search = null, decimal? specialtyId = null, string? sortBy = null, decimal? maxFee = null);
        Task<IEnumerable<Doctor>> GetFeaturedDoctorsAsync();
        Task<Doctor?> GetDoctorDetailsAsync(decimal doctorId);
        Task<decimal> CreateDoctorAsync(DoctorUpsertDto dto);
        Task<bool> UpdateDoctorAsync(decimal doctorId, DoctorUpsertDto dto);
        Task<bool> DeleteDoctorAsync(decimal doctorId);

        Task<IEnumerable<DoctorWorkingHours>> GetWorkingHoursAsync(decimal doctorId);
        Task<decimal> AddWorkingHoursAsync(WorkingHoursUpsertDto dto);
        Task<bool> UpdateWorkingHoursAsync(decimal workingHoursId, WorkingHoursUpsertDto dto);
        Task<bool> DeleteWorkingHoursAsync(decimal workingHoursId);

        Task<IEnumerable<AppointmentSlot>> GetDoctorSlotsAsync(decimal doctorId, DateTime? date = null, bool availableOnly = false);
        Task<decimal> AddSlotAsync(SlotCreateDto dto);
        Task<bool> UpdateSlotStatusAsync(decimal slotId, bool isAvailable);
        Task<bool> UpdateSlotAsync(decimal slotId, SlotFullUpdateDto dto);
        Task<bool> DeleteSlotAsync(decimal slotId);
    }

    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetPatientsAsync(string? search = null);
        Task<Patient?> GetPatientByIdAsync(decimal patientId);
        Task<decimal> RegisterPatientAsync(RegisterPatientRequest request);
        Task<bool> UpdatePatientAsync(decimal patientId, PatientUpsertDto dto);
        Task<bool> DeletePatientAsync(decimal patientId);
    }

    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsAsync(string? search = null, DateTime? date = null, string? status = null, decimal? doctorId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Appointment?> GetAppointmentByIdAsync(decimal appointmentId);
        Task<Appointment?> GetAppointmentByReferenceAsync(string referenceNumber);
        Task<BookAppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request);
        Task<bool> CancelAppointmentAsync(decimal appointmentId);
        Task<bool> UpdateAppointmentStatusAsync(decimal appointmentId, string status);
    }

    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }

    public interface IAdminService
    {
        Task<decimal> AddAdminAsync(Admin admin);
        Task<(decimal? AdminId, bool IsSuccess, string? RoleName)> LoginAsync(string adminName, string password);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> DeleteAdminAsync(decimal adminId);
        Task<bool> DeleteAdminByNameAsync(string adminName);
        Task<int> GetActiveAdminCountAsync();
        Task<Admin?> GetAdminByIdAsync(decimal adminId);
        Task<AdminResponseDto?> GetAdminDetailsByIdAsync(decimal adminId);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
    }

    public interface IDashboardService
    {
        Task<AdminDashboardStats> GetDashboardStatsAsync();
        Task<IEnumerable<SpecialtyAppointmentChartItem>> GetSpecialtyChartDataAsync();
    }
}
