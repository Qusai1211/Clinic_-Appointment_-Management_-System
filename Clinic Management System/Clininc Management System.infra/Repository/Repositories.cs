using Clininc_Management_System.Core.common;
using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Clininc_Management_System.infra.Repository
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly IDbContext _dbContext;

        public SpecialtyRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            const string sql = @"
                SELECT s.SpecialtyId, s.SpecialtyName, s.Description,
                       (SELECT COUNT(*) FROM Doctors d WHERE d.SpecialtyId = s.SpecialtyId) AS DoctorCount,
                       (SELECT COUNT(*) FROM Appointments a JOIN Doctors d ON a.DoctorId = d.DoctorId WHERE d.SpecialtyId = s.SpecialtyId) AS AppointmentCount
                FROM Specialties s
                ORDER BY s.SpecialtyName";

            return await _dbContext.connection1.QueryAsync<Specialty>(sql);
        }

        public async Task<Specialty?> GetByIdAsync(decimal specialtyId)
        {
            const string sql = @"
                SELECT s.SpecialtyId, s.SpecialtyName, s.Description,
                       (SELECT COUNT(*) FROM Doctors d WHERE d.SpecialtyId = s.SpecialtyId) AS DoctorCount,
                       (SELECT COUNT(*) FROM Appointments a JOIN Doctors d ON a.DoctorId = d.DoctorId WHERE d.SpecialtyId = s.SpecialtyId) AS AppointmentCount
                FROM Specialties s
                WHERE s.SpecialtyId = :SpecialtyId";

            return await _dbContext.connection1.QueryFirstOrDefaultAsync<Specialty>(sql, new { SpecialtyId = specialtyId });
        }

        public async Task<decimal> CreateAsync(SpecialtyUpsertDto dto)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Specialties (SpecialtyName, Description)
                VALUES (:SpecialtyName, :Description)
                RETURNING SpecialtyId INTO :NewId";

            cmd.Parameters.Add("SpecialtyName", OracleDbType.Varchar2).Value = dto.SpecialtyName;
            cmd.Parameters.Add("Description", OracleDbType.Varchar2).Value = (object?)dto.Description ?? DBNull.Value;

            var outParam = new OracleParameter("NewId", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value;
        }

        public async Task<bool> UpdateAsync(decimal specialtyId, SpecialtyUpsertDto dto)
        {
            const string sql = @"
                UPDATE Specialties 
                SET SpecialtyName = :SpecialtyName, Description = :Description
                WHERE SpecialtyId = :SpecialtyId";

            var rows = await _dbContext.connection1.ExecuteAsync(sql, new
            {
                SpecialtyName = dto.SpecialtyName,
                Description = dto.Description,
                SpecialtyId = specialtyId
            });

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(decimal specialtyId)
        {
            const string sql = "DELETE FROM Specialties WHERE SpecialtyId = :SpecialtyId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new { SpecialtyId = specialtyId });
            return rows > 0;
        }
    }

    public class DoctorRepository : IDoctorRepository
    {
        private readonly IDbContext _dbContext;

        public DoctorRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync(string? search = null, decimal? specialtyId = null, string? sortBy = null)
        {
            var sql = @"
                SELECT d.DoctorId, d.FullName, d.Qualifications, d.ConsultationFee, 
                       d.ProfileImageUrl, d.IsFeatured, d.SpecialtyId, s.SpecialtyName
                FROM Doctors d
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (LOWER(d.FullName) LIKE :Search OR LOWER(d.Qualifications) LIKE :Search OR LOWER(s.SpecialtyName) LIKE :Search)";
                parameters.Add("Search", $"%{search.Trim().ToLower()}%");
            }

            if (specialtyId.HasValue && specialtyId.Value > 0)
            {
                sql += " AND d.SpecialtyId = :SpecialtyId";
                parameters.Add("SpecialtyId", specialtyId.Value);
            }

            if (sortBy?.ToLower() == "name_desc")
            {
                sql += " ORDER BY d.FullName DESC";
            }
            else if (sortBy?.ToLower() == "specialty")
            {
                sql += " ORDER BY s.SpecialtyName ASC, d.FullName ASC";
            }
            else if (sortBy?.ToLower() == "fee_asc")
            {
                sql += " ORDER BY d.ConsultationFee ASC";
            }
            else if (sortBy?.ToLower() == "fee_desc")
            {
                sql += " ORDER BY d.ConsultationFee DESC";
            }
            else
            {
                sql += " ORDER BY d.FullName ASC";
            }

            return await _dbContext.connection1.QueryAsync<Doctor>(sql, parameters);
        }

        public async Task<IEnumerable<Doctor>> GetFeaturedAsync()
        {
            const string sql = @"
                SELECT d.DoctorId, d.FullName, d.Qualifications, d.ConsultationFee, 
                       d.ProfileImageUrl, d.IsFeatured, d.SpecialtyId, s.SpecialtyName
                FROM Doctors d
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                WHERE d.IsFeatured = 1
                ORDER BY d.DoctorId ASC";

            return await _dbContext.connection1.QueryAsync<Doctor>(sql);
        }

        public async Task<Doctor?> GetByIdAsync(decimal doctorId)
        {
            const string sql = @"
                SELECT d.DoctorId, d.FullName, d.Qualifications, d.ConsultationFee, 
                       d.ProfileImageUrl, d.IsFeatured, d.SpecialtyId, s.SpecialtyName
                FROM Doctors d
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                WHERE d.DoctorId = :DoctorId";

            var doctor = await _dbContext.connection1.QueryFirstOrDefaultAsync<Doctor>(sql, new { DoctorId = doctorId });
            if (doctor == null) return null;

            doctor.WorkingHours = (await GetWorkingHoursByDoctorAsync(doctorId)).ToList();
            doctor.AvailableSlots = (await GetSlotsByDoctorAsync(doctorId, availableOnly: true)).ToList();

            return doctor;
        }

        public async Task<decimal> CreateAsync(DoctorUpsertDto dto)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Doctors (FullName, Qualifications, ConsultationFee, ProfileImageUrl, IsFeatured, SpecialtyId)
                VALUES (:FullName, :Qualifications, :ConsultationFee, :ProfileImageUrl, :IsFeatured, :SpecialtyId)
                RETURNING DoctorId INTO :NewId";

            cmd.Parameters.Add("FullName", OracleDbType.Varchar2).Value = dto.FullName;
            cmd.Parameters.Add("Qualifications", OracleDbType.Varchar2).Value = (object?)dto.Qualifications ?? DBNull.Value;
            cmd.Parameters.Add("ConsultationFee", OracleDbType.Decimal).Value = dto.ConsultationFee;
            cmd.Parameters.Add("ProfileImageUrl", OracleDbType.Varchar2).Value = (object?)dto.ProfileImageUrl ?? DBNull.Value;
            cmd.Parameters.Add("IsFeatured", OracleDbType.Int32).Value = dto.IsFeatured;
            cmd.Parameters.Add("SpecialtyId", OracleDbType.Decimal).Value = (object?)dto.SpecialtyId ?? DBNull.Value;

            var outParam = new OracleParameter("NewId", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value;
        }

        public async Task<bool> UpdateAsync(decimal doctorId, DoctorUpsertDto dto)
        {
            const string sql = @"
                UPDATE Doctors 
                SET FullName = :FullName,
                    Qualifications = :Qualifications,
                    ConsultationFee = :ConsultationFee,
                    ProfileImageUrl = :ProfileImageUrl,
                    IsFeatured = :IsFeatured,
                    SpecialtyId = :SpecialtyId
                WHERE DoctorId = :DoctorId";

            var rows = await _dbContext.connection1.ExecuteAsync(sql, new
            {
                FullName = dto.FullName,
                Qualifications = dto.Qualifications,
                ConsultationFee = dto.ConsultationFee,
                ProfileImageUrl = dto.ProfileImageUrl,
                IsFeatured = dto.IsFeatured,
                SpecialtyId = dto.SpecialtyId,
                DoctorId = doctorId
            });

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(decimal doctorId)
        {
            const string sql = "DELETE FROM Doctors WHERE DoctorId = :DoctorId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new { DoctorId = doctorId });
            return rows > 0;
        }

        public async Task<IEnumerable<DoctorWorkingHours>> GetWorkingHoursByDoctorAsync(decimal doctorId)
        {
            const string sql = @"
                SELECT WorkingHoursId, DayOfWeek, StartTime, EndTime, DoctorId
                FROM DoctorWorkingHours
                WHERE DoctorId = :DoctorId
                ORDER BY WorkingHoursId ASC";

            return await _dbContext.connection1.QueryAsync<DoctorWorkingHours>(sql, new { DoctorId = doctorId });
        }

        public async Task<decimal> AddWorkingHoursAsync(WorkingHoursUpsertDto dto)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandText = @"
                INSERT INTO DoctorWorkingHours (DayOfWeek, StartTime, EndTime, DoctorId)
                VALUES (:DayOfWeek, :StartTime, :EndTime, :DoctorId)
                RETURNING WorkingHoursId INTO :NewId";

            cmd.Parameters.Add("DayOfWeek", OracleDbType.Varchar2).Value = dto.DayOfWeek;
            cmd.Parameters.Add("StartTime", OracleDbType.Varchar2).Value = dto.StartTime;
            cmd.Parameters.Add("EndTime", OracleDbType.Varchar2).Value = dto.EndTime;
            cmd.Parameters.Add("DoctorId", OracleDbType.Decimal).Value = dto.DoctorId;

            var outParam = new OracleParameter("NewId", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value;
        }

        public async Task<bool> DeleteWorkingHoursAsync(decimal workingHoursId)
        {
            const string sql = "DELETE FROM DoctorWorkingHours WHERE WorkingHoursId = :WorkingHoursId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new { WorkingHoursId = workingHoursId });
            return rows > 0;
        }

        public async Task<IEnumerable<AppointmentSlot>> GetSlotsByDoctorAsync(decimal doctorId, DateTime? date = null, bool availableOnly = false)
        {
            var sql = @"
                SELECT s.SlotId, s.SlotDate, s.SlotTime, s.IsAvailable, s.DoctorId, d.FullName AS DoctorName
                FROM AppointmentSlots s
                JOIN Doctors d ON s.DoctorId = d.DoctorId
                WHERE s.DoctorId = :DoctorId";

            var parameters = new DynamicParameters();
            parameters.Add("DoctorId", doctorId);

            if (date.HasValue)
            {
                sql += " AND TRUNC(s.SlotDate) = TRUNC(:SlotDate)";
                parameters.Add("SlotDate", date.Value.Date);
            }
            else
            {
                sql += " AND TRUNC(s.SlotDate) >= TRUNC(SYSDATE)";
            }

            if (availableOnly)
            {
                sql += " AND s.IsAvailable = 1";
            }

            sql += " ORDER BY s.SlotDate ASC, s.SlotTime ASC";

            return await _dbContext.connection1.QueryAsync<AppointmentSlot>(sql, parameters);
        }

        public async Task<decimal> AddSlotAsync(SlotCreateDto dto)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandText = @"
                INSERT INTO AppointmentSlots (SlotDate, SlotTime, IsAvailable, DoctorId)
                VALUES (TRUNC(:SlotDate), :SlotTime, 1, :DoctorId)
                RETURNING SlotId INTO :NewId";

            cmd.Parameters.Add("SlotDate", OracleDbType.Date).Value = dto.SlotDate.Date;
            cmd.Parameters.Add("SlotTime", OracleDbType.Varchar2).Value = dto.SlotTime;
            cmd.Parameters.Add("DoctorId", OracleDbType.Decimal).Value = dto.DoctorId;

            var outParam = new OracleParameter("NewId", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value;
        }

        public async Task<bool> DeleteSlotAsync(decimal slotId)
        {
            const string sql = "DELETE FROM AppointmentSlots WHERE SlotId = :SlotId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new { SlotId = slotId });
            return rows > 0;
        }
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly IDbContext _dbContext;

        public PatientRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync(string? search = null)
        {
            var sql = "SELECT PatientId, FullName, PhoneNumber, Email, DateOfBirth FROM Patients WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (LOWER(FullName) LIKE :Search OR PhoneNumber LIKE :Search OR LOWER(Email) LIKE :Search)";
                parameters.Add("Search", $"%{search.Trim().ToLower()}%");
            }

            sql += " ORDER BY PatientId DESC";
            return await _dbContext.connection1.QueryAsync<Patient>(sql, parameters);
        }

        public async Task<Patient?> GetByIdAsync(decimal patientId)
        {
            const string sql = "SELECT PatientId, FullName, PhoneNumber, Email, DateOfBirth FROM Patients WHERE PatientId = :PatientId";
            return await _dbContext.connection1.QueryFirstOrDefaultAsync<Patient>(sql, new { PatientId = patientId });
        }

        public async Task<decimal> RegisterPatientAsync(RegisterPatientRequest request)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PKG_CLINIC_OPERATIONS.REGISTER_PATIENT";

            cmd.Parameters.Add("p_FullName", OracleDbType.Varchar2).Value = request.FullName;
            cmd.Parameters.Add("p_PhoneNumber", OracleDbType.Varchar2).Value = request.PhoneNumber;
            cmd.Parameters.Add("p_Email", OracleDbType.Varchar2).Value = request.Email;
            cmd.Parameters.Add("p_DateOfBirth", OracleDbType.Date).Value = request.DateOfBirth.Date;

            var outParam = new OracleParameter("o_PatientId", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)outParam.Value).Value;
        }
    }

    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbContext _dbContext;

        public AppointmentRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(string? search = null, DateTime? date = null, string? status = null)
        {
            var sql = @"
                SELECT a.AppointmentId, a.ReferenceNumber, a.AppointmentDate, a.AppointmentTime, 
                       a.Status, a.DoctorId, d.FullName AS DoctorName, s.SpecialtyName,
                       a.SlotId, a.PatientId, p.FullName AS PatientName, p.PhoneNumber AS PatientPhone, 
                       p.Email AS PatientEmail, d.ConsultationFee
                FROM Appointments a
                LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                LEFT JOIN Patients p ON a.PatientId = p.PatientId
                WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (LOWER(p.FullName) LIKE :Search OR LOWER(d.FullName) LIKE :Search OR LOWER(a.ReferenceNumber) LIKE :Search)";
                parameters.Add("Search", $"%{search.Trim().ToLower()}%");
            }

            if (date.HasValue)
            {
                sql += " AND TRUNC(a.AppointmentDate) = TRUNC(:AppDate)";
                parameters.Add("AppDate", date.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
            {
                sql += " AND LOWER(a.Status) = LOWER(:Status)";
                parameters.Add("Status", status.Trim());
            }

            sql += " ORDER BY a.AppointmentDate DESC, a.AppointmentTime ASC, a.AppointmentId DESC";

            return await _dbContext.connection1.QueryAsync<Appointment>(sql, parameters);
        }

        public async Task<Appointment?> GetByIdAsync(decimal appointmentId)
        {
            const string sql = @"
                SELECT a.AppointmentId, a.ReferenceNumber, a.AppointmentDate, a.AppointmentTime, 
                       a.Status, a.DoctorId, d.FullName AS DoctorName, s.SpecialtyName,
                       a.SlotId, a.PatientId, p.FullName AS PatientName, p.PhoneNumber AS PatientPhone, 
                       p.Email AS PatientEmail, d.ConsultationFee
                FROM Appointments a
                LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                LEFT JOIN Patients p ON a.PatientId = p.PatientId
                WHERE a.AppointmentId = :AppointmentId";

            return await _dbContext.connection1.QueryFirstOrDefaultAsync<Appointment>(sql, new { AppointmentId = appointmentId });
        }

        public async Task<Appointment?> GetByReferenceNumberAsync(string referenceNumber)
        {
            const string sql = @"
                SELECT a.AppointmentId, a.ReferenceNumber, a.AppointmentDate, a.AppointmentTime, 
                       a.Status, a.DoctorId, d.FullName AS DoctorName, s.SpecialtyName,
                       a.SlotId, a.PatientId, p.FullName AS PatientName, p.PhoneNumber AS PatientPhone, 
                       p.Email AS PatientEmail, d.ConsultationFee
                FROM Appointments a
                LEFT JOIN Doctors d ON a.DoctorId = d.DoctorId
                LEFT JOIN Specialties s ON d.SpecialtyId = s.SpecialtyId
                LEFT JOIN Patients p ON a.PatientId = p.PatientId
                WHERE LOWER(a.ReferenceNumber) = LOWER(:ReferenceNumber)";

            return await _dbContext.connection1.QueryFirstOrDefaultAsync<Appointment>(sql, new { ReferenceNumber = referenceNumber.Trim() });
        }

        public async Task<(decimal AppointmentId, string ReferenceNumber)> BookAppointmentAsync(BookAppointmentRequest request)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PKG_CLINIC_OPERATIONS.BOOK_APPOINTMENT";

            cmd.Parameters.Add("p_DoctorId", OracleDbType.Decimal).Value = request.DoctorId;
            cmd.Parameters.Add("p_PatientId", OracleDbType.Decimal).Value = request.PatientId!.Value;
            cmd.Parameters.Add("p_SlotId", OracleDbType.Decimal).Value = request.SlotId;
            cmd.Parameters.Add("p_AppointmentDate", OracleDbType.Date).Value = request.AppointmentDate.Date;
            cmd.Parameters.Add("p_AppointmentTime", OracleDbType.Varchar2).Value = request.AppointmentTime;

            var outAppId = new OracleParameter("o_AppointmentId", OracleDbType.Decimal, ParameterDirection.Output);
            var outRefNum = new OracleParameter("o_ReferenceNumber", OracleDbType.Varchar2, 100, null, ParameterDirection.Output);

            cmd.Parameters.Add(outAppId);
            cmd.Parameters.Add(outRefNum);

            await cmd.ExecuteNonQueryAsync();

            var appId = ((Oracle.ManagedDataAccess.Types.OracleDecimal)outAppId.Value).Value;
            var refNum = outRefNum.Value?.ToString() ?? string.Empty;

            return (appId, refNum);
        }

        public async Task<bool> CancelAppointmentAsync(decimal appointmentId)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PKG_CLINIC_OPERATIONS.CANCEL_APPOINTMENT";

            cmd.Parameters.Add("p_AppointmentId", OracleDbType.Decimal).Value = appointmentId;

            await cmd.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(decimal appointmentId, string status)
        {
            using var cmd = (OracleCommand)((OracleConnection)_dbContext.connection1).CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PKG_CLINIC_OPERATIONS.UPDATE_APPOINTMENT_STATUS";

            cmd.Parameters.Add("p_AppointmentId", OracleDbType.Decimal).Value = appointmentId;
            cmd.Parameters.Add("p_Status", OracleDbType.Varchar2).Value = status;

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
    }

    public class AdminRepository : IAdminRepository
    {
        private readonly IDbContext _dbContext;

        public AdminRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT AdminId, AdminName, PasswordHash
                FROM Admins
                WHERE LOWER(AdminName) = LOWER(:Username)";

            return await _dbContext.connection1.QueryFirstOrDefaultAsync<Admin>(sql, new { Username = username.Trim() });
        }
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbContext _dbContext;

        public DashboardRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AdminDashboardStats> GetStatsAsync()
        {
            const string sql = @"
                SELECT TotalDoctors, TotalPatients, TotalAppointments, 
                       ScheduledAppointments, CompletedAppointments, CancelledAppointments
                FROM Vw_AdminDashboardStats";

            return await _dbContext.connection1.QueryFirstOrDefaultAsync<AdminDashboardStats>(sql) ?? new AdminDashboardStats();
        }

        public async Task<IEnumerable<SpecialtyAppointmentChartItem>> GetAppointmentsBySpecialtyAsync()
        {
            const string sql = @"
                SELECT SpecialtyId, SpecialtyName, AppointmentCount
                FROM Vw_AppointmentsBySpecialty
                ORDER BY SpecialtyName ASC";

            return await _dbContext.connection1.QueryAsync<SpecialtyAppointmentChartItem>(sql);
        }
    }
}
