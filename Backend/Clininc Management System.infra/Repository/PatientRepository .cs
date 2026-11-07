using Clininc_Management_System.Core.common;
using Clininc_Management_System.Core.DTO;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Repository
{
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

        public async Task<bool> UpdateAsync(decimal patientId, PatientUpsertDto dto)
        {
            const string sql = @"
                UPDATE Patients
                SET FullName = :FullName, PhoneNumber = :PhoneNumber, Email = :Email, DateOfBirth = TRUNC(:DateOfBirth)
                WHERE PatientId = :PatientId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth.Date,
                PatientId = patientId
            });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(decimal patientId)
        {
            const string sql = "DELETE FROM Patients WHERE PatientId = :PatientId";
            var rows = await _dbContext.connection1.ExecuteAsync(sql, new { PatientId = patientId });
            return rows > 0;
        }
    }
}
