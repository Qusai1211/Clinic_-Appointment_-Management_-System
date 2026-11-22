using Clinic_Managemeant.Core.common;
using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
using Clinic_Managemeant.Core.Service;
using Clinic_Management.Infra.Common;
using Dapper;
using Dapper.Oracle;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbContext _context;
        public PatientRepository(IDbContext context)
        {
            _context = context;
        }
        public async Task<long> AddPatientAsync(Patient patient)
        {

            var parameters = new DynamicParameters();
            parameters.Add("p_FullName", patient.Fullname, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_PhoneNumber", patient.Phonenumber, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_Email", patient.Email, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_DateOfBirth", patient.Dateofbirth, dbType: DbType.Date, direction: ParameterDirection.Input);
            parameters.Add("o_PatientId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            await _context.Connection.ExecuteAsync(
                "PKG_PATIENTS_MNG.ADD_PATIENT",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            long? patientId = parameters.Get<long?>("o_PatientId");
            return patientId ?? 0;
        }
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_PatientId", patient.Patientid, dbType: DbType.Int64, direction: ParameterDirection.Input);
            parameters.Add("p_FullName", patient.Fullname, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_PhoneNumber", patient.Phonenumber, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_Email", patient.Email, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_DateOfBirth", patient.Dateofbirth, dbType: DbType.Date, direction: ParameterDirection.Input);

            await _context.Connection.ExecuteAsync(
                "PKG_PATIENTS_MNG.UPDATE_PATIENT",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_PatientId", patientId, dbType: DbType.Int32, direction: ParameterDirection.Input);
              await _context.Connection.ExecuteAsync(
                "PKG_PATIENTS_MNG.DELETE_PATIENT",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return  true;
        }
        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_PatientId", patientId,dbType: OracleMappingType.Decimal, direction: ParameterDirection.Input);
            parameters.Add("o_Cursor",  dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            var result = await _context.Connection.QueryFirstOrDefaultAsync<Patient>(
                "PKG_PATIENTS_MNG.GET_PATIENT_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return result;

        }
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("o_Cursor", dbType:OracleMappingType.RefCursor , direction: ParameterDirection.Output);
            IEnumerable<Patient> result = await _context.Connection.QueryAsync<Patient>(
                "PKG_PATIENTS_MNG.GET_ALL_PATIENTS",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

       
    }
}
