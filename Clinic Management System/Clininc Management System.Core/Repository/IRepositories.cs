using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Models;

namespace Clininc_Management_System.Core.Repository
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<Specialty?> GetByIdAsync(decimal specialtyId);
        Task<decimal> CreateAsync(SpecialtyUpsertDto dto);
        Task<bool> UpdateAsync(decimal specialtyId, SpecialtyUpsertDto dto);
        Task<bool> DeleteAsync(decimal specialtyId);
    }

    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync(string? search = null, decimal? specialtyId = null, string? sortBy = null);
        Task<IEnumerable<Doctor>> GetFeaturedAsync();
        Task<Doctor?> GetByIdAsync(decimal doctorId);
        Task<decimal> CreateAsync(DoctorUpsertDto dto);
        Task<bool> UpdateAsync(decimal doctorId, DoctorUpsertDto dto);
        Task<bool> DeleteAsync(decimal doctorId);

        Task<IEnumerable<DoctorWorkingHours>> GetWorkingHoursByDoctorAsync(decimal doctorId);
        Task<decimal> AddWorkingHoursAsync(WorkingHoursUpsertDto dto);
        Task<bool> UpdateWorkingHoursAsync(decimal workingHoursId, WorkingHoursUpsertDto dto);
        Task<bool> DeleteWorkingHoursAsync(decimal workingHoursId);

        Task<IEnumerable<AppointmentSlot>> GetSlotsByDoctorAsync(decimal doctorId, DateTime? date = null, bool availableOnly = false);
        Task<decimal> AddSlotAsync(SlotCreateDto dto);
        Task<bool> UpdateSlotStatusAsync(decimal slotId, bool isAvailable);
        Task<bool> DeleteSlotAsync(decimal slotId);
    }

    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync(string? search = null);
        Task<Patient?> GetByIdAsync(decimal patientId);
        Task<decimal> RegisterPatientAsync(RegisterPatientRequest request);
        Task<bool> UpdateAsync(decimal patientId, PatientUpsertDto dto);
        Task<bool> DeleteAsync(decimal patientId);
    }

    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync(string? search = null, DateTime? date = null, string? status = null);
        Task<Appointment?> GetByIdAsync(decimal appointmentId);
        Task<Appointment?> GetByReferenceNumberAsync(string referenceNumber);
        Task<(decimal AppointmentId, string ReferenceNumber)> BookAppointmentAsync(BookAppointmentRequest request);
        Task<bool> CancelAppointmentAsync(decimal appointmentId);
        Task<bool> UpdateStatusAsync(decimal appointmentId, string status);
    }

    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task<bool> UpdatePasswordHashAsync(decimal adminId, string newPasswordHash);
    }

    public interface IDashboardRepository
    {
        Task<AdminDashboardStats> GetStatsAsync();
        Task<IEnumerable<SpecialtyAppointmentChartItem>> GetAppointmentsBySpecialtyAsync();
    }
}

