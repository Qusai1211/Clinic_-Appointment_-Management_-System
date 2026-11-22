using Clinic_Managemeant.Core.common;
using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Repository;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clinic_Management.Infra.Repository
{
    public class DoctorRepository: IDoctorRepository
    {
        private readonly IDbContext _dbContext;
        public DoctorRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("o_Cursor", dbType:OracleMappingType.RefCursor, direction: ParameterDirection.Output);
             IEnumerable<Doctor> doctors = await _dbContext.Connection.QueryAsync<Doctor>(
                "PKG_DOCTORS_MNG.GET_ALL_DOCTORS",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return doctors.ToList();

        }
        public async Task<long> AddDoctorAsync(Doctor doctor)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_FullName", doctor.FullName, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_Qualifications", doctor.Qualifications, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_ConsultationFee", doctor.ConsultationFee, dbType: DbType.Decimal, direction: ParameterDirection.Input);
            parameters.Add("p_ProfileImageUrl", doctor.ProfileImageUrl, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_IsFeatured", (bool)doctor.IsFeatured ? 1 : 0, dbType: DbType.Int64, direction: ParameterDirection.Input);
            parameters.Add("p_SpecialtyId", doctor.SpecialtyId, dbType: DbType.Int64, direction: ParameterDirection.Input);
            parameters.Add("o_DoctorId", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await _dbContext.Connection.ExecuteAsync(
                "PKG_DOCTORS_MNG.ADD_DOCTOR",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            long? doctorId = parameters.Get<long?>("o_DoctorId");
            return doctorId ?? 0;
        }
        
      
        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            var parameter= new DynamicParameters();

            parameter.Add("p_SpecialtyId", doctor.SpecialtyId,
                dbType: DbType.Int64); parameter.Add("p_DoctorId", doctor.DoctorId, dbType: DbType.Int64, direction: ParameterDirection.Input);
            parameter.Add("p_FullName", doctor.FullName, dbType: DbType.String, direction: ParameterDirection.Input);
            parameter.Add("p_Qualifications", doctor.Qualifications, dbType: DbType.String, direction: ParameterDirection.Input);
            parameter.Add("p_ConsultationFee", doctor.ConsultationFee, dbType: DbType.Decimal, direction: ParameterDirection.Input);
            parameter.Add("p_ProfileImageUrl", doctor.ProfileImageUrl, dbType: DbType.String, direction: ParameterDirection.Input);
            parameter.Add("p_IsFeatured", (bool)doctor.IsFeatured ? 1 : 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameter.Add("p_SpecialtyId", doctor.SpecialtyId, dbType: DbType.Int64, direction: ParameterDirection.Input);
            await _dbContext.Connection.ExecuteAsync(
                "PKG_DOCTORS_MNG.UPDATE_DOCTOR",
                parameter,
                commandType: CommandType.StoredProcedure
            );
            return true ;
        }
       

public async Task<bool> DeleteDoctorAsync(int doctorId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("p_DoctorId", doctorId, dbType: DbType.Int32, direction: ParameterDirection.Input);

        try
        {
            await _dbContext.Connection.ExecuteAsync(
                "PKG_DOCTORS_MNG.DELETE_DOCTOR",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return true;
        }
        catch (OracleException ex) when (ex.Number == 20013)   // Doctor not found
        {
            return false;
        }
    }
    public async Task<Doctor?> GetDoctorAsync(int doctorId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("p_DoctorId", doctorId, dbType:OracleMappingType.Decimal , direction: ParameterDirection.Input);
            parameters.Add("o_Cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            var doctor = await _dbContext.Connection.QueryFirstOrDefaultAsync<Doctor>(
                "PKG_DOCTORS_MNG.GET_DOCTOR_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return doctor;
        }

    }
}
