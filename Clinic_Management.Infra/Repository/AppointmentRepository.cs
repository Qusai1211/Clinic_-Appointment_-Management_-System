using Clinic_Managemeant.Core.common;
using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Dapper;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(IDbContext dbContext, ILogger<AppointmentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                using var connection = _dbContext.Connection;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                string sql = "SELECT * FROM Appointments";
                var result = await connection.QueryAsync<Appointment>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء جلب قائمة المواعيد.");
                throw;
            }
        }

        public async Task<long> AddAppointmentAsync(Appointment appointment)
        {
            try
            {
                using var connection = _dbContext.Connection;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var parameters = new DynamicParameters();
                // مطابقة المعاملات مع Stored Procedure: PKG_CLINIC_OPERATIONS.BOOK_APPOINTMENT
                parameters.Add("p_DoctorId", appointment.Doctorid, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_PatientId", appointment.Patientid, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_SlotId", appointment.Slotid, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_AppointmentDate", appointment.Appointmentdate, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_AppointmentTime", appointment.Appointmenttime, DbType.String, ParameterDirection.Input);

                // معاملات الإخراج OUT
                parameters.Add("o_AppointmentId", dbType: DbType.Int64, direction: ParameterDirection.Output);
                parameters.Add("o_ReferenceNumber", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "PKG_CLINIC_OPERATIONS.BOOK_APPOINTMENT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                long newAppointmentId = parameters.Get<long>("o_AppointmentId");
                return newAppointmentId;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "خطأ قاعدة بيانات أثناء حجز الموعد للمريض: {PatientId}", appointment.Patientid);
                throw new Exception($"خطأ في Oracle: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ غير متوقع أثناء حجز الموعد.");
                throw;
            }
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            try
            {
                using var connection = _dbContext.Connection;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_AppointmentId", appointment.Appointmentid, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_Status", appointment.Status ?? "Scheduled", DbType.String, ParameterDirection.Input);

                await connection.ExecuteAsync(
                    "PKG_CLINIC_OPERATIONS.UPDATE_APPOINTMENT_STATUS",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء تحديث حالة الموعد: {AppointmentId}", appointment.Appointmentid);
                throw;
            }
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {
            try
            {
                using var connection = _dbContext.Connection;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("p_AppointmentId", appointmentId, DbType.Int32, ParameterDirection.Input);

                await connection.ExecuteAsync(
                    "PKG_CLINIC_OPERATIONS.CANCEL_APPOINTMENT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء إلغاء الموعد: {AppointmentId}", appointmentId);
                throw;
            }
        }

        public async Task DeleteAppointmentAsync(Appointment appointment)
        {
            if (appointment != null)
            {
                await DeleteAppointmentAsync(appointment);
            }
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            try
            {
                using var connection = _dbContext.Connection;
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                string sql = "SELECT * FROM Appointments WHERE Appointmentid = :Appointmentid";
                var parameters = new DynamicParameters();
                parameters.Add("Appointmentid", appointmentId, DbType.Int32, ParameterDirection.Input);

                var result = await connection.QueryFirstOrDefaultAsync<Appointment>(sql, parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ أثناء استرجاع الموعد: {AppointmentId}", appointmentId);
                throw;
            }
        }
    }
}