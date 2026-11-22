
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
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IDbContext idbcontext;

        public AdminRepository(IDbContext dbContext)
        {
            idbcontext = dbContext;
        }

       
        // إضافة مشرف جديد مع دعم التزامن
        public async Task<long> AddAdminAsync(Admin admin)
        {
            var parameters = new DynamicParameters();

            parameters.Add("p_AdminName", admin.Adminname, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_PasswordHash", admin.Passwordhash, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_RoleId", 1, dbType: DbType.Int32, direction: ParameterDirection.Input); // الدور الافتراضي 1
            parameters.Add("p_Email", admin.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("o_AdminId", dbType: DbType.Int64, direction: ParameterDirection.Output);

            // تنفيذ الإجراء المخزن بشكل غير متزامن (Async) لعدم حجز خيوط المعالجة
            await idbcontext.Connection.ExecuteAsync(
                "PKG_ADMINS_MNG.ADD_ADMIN",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return Convert.ToInt64(parameters.Get<object>("o_AdminId").ToString());
        }
        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            var parameters = new DynamicParameters();

            parameters.Add("p_AdminId", admin.Adminid, dbType: DbType.Int64, direction: ParameterDirection.Input);
            parameters.Add("p_AdminName", admin.Adminname ?.Trim(), dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_PasswordHash", admin.Passwordhash, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("p_RoleId", admin.Roleid, dbType: DbType.Int32, direction: ParameterDirection.Input); parameters.Add("p_IsActive",  (bool)(admin.Isactive ?? true) ? 1 : 0, dbType: DbType.Int32, direction: ParameterDirection.Input);
            await idbcontext.Connection.ExecuteAsync(
                "PKG_ADMINS_MNG.UPDATE_ADMIN",
                parameters,
                commandType: CommandType.StoredProcedure
            ); 

            return true;
        }

        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("p_AdminId", adminId, dbType: DbType.Int32, direction: ParameterDirection.Input);

            await idbcontext.Connection.ExecuteAsync(
                "PKG_ADMINS_MNG.DELETE_ADMIN_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<(long? AdminId, bool IsSuccess, string? RoleName)> LoginAsync(string adminName, string passwordHash)
        {
            var parameters = new DynamicParameters();

            parameters.Add("p_AdminName", adminName.Trim(), DbType.String, ParameterDirection.Input);
            parameters.Add("p_PasswordHash", passwordHash, DbType.String, ParameterDirection.Input);

            // معاملات الإخراج (OUT Parameters)
            parameters.Add("o_AdminId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("o_RoleName", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
            parameters.Add("o_IsSuccess", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await idbcontext.Connection.ExecuteAsync(
                "PKG_ADMINS_MNG.LOGIN_ADMIN",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var isSuccess = Convert.ToInt32(parameters.Get<object>("o_IsSuccess").ToString()) == 1;
            if (!isSuccess) return (null, false, null);

            var adminId = Convert.ToInt64(parameters.Get<object>("o_AdminId").ToString());
            var roleName = parameters.Get<object>("o_RoleName")?.ToString();

            return (adminId, isSuccess, roleName);
        }


        public async Task<List<Admin>> GetAllAdminsAsync()
        {
            var parameters = new OracleDynamicParameters();

            parameters.Add("o_Cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

          IEnumerable<Admin>  admins = await idbcontext.Connection.QueryAsync<Admin>(
                "PKG_ADMINS_MNG.GET_ALL_ADMINS",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return admins.ToList(); // تحويل النتيجة إلى قائمة
        }
        public async Task<Admin> GetAdminByIdAsync(int adminId)
        {
            var parameters = new OracleDynamicParameters();

            parameters.Add("p_AdminId", adminId, dbType: OracleMappingType.Decimal, direction: ParameterDirection.Input);
            parameters.Add("o_Cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            var admin = await idbcontext.Connection.QueryFirstOrDefaultAsync<Admin>(
                "PKG_ADMINS_MNG.GET_ADMIN_BY_ID",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return admin;
        }


        
    }
}
