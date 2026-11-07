using Clininc_Management_System.Core.common;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IDbContext dbContext;

        public AdminRepository(IDbContext dbContext1)
        {
            dbContext = dbContext1;
        }

        // جلب كل المشرفين (Async)
        public async Task<List<Admin>> GetAllAdmins()
        {
            String sql = "SELECT * FROM ADMINS";
            var result = await dbContext.connection1.QueryAsync<Admin>(sql);
            return result.ToList();
        }
        public async Task<Admin?> GetAdminByUsername(string username)
        {
            string sql = "SELECT * FROM ADMINS WHERE Adminname = :Adminname";
            var parameter = new DynamicParameters();
            parameter.Add("Adminname", username);

            var result = await dbContext.connection1.QueryFirstOrDefaultAsync<Admin>(sql, parameter);
            return result;
        }

        // إضافة مشرف جديد (Async)
        public async Task creatAdmin(Admin admin)
        {
            String sql = "INSERT INTO ADMINS (AdminId, Adminname, Passwordhash) VALUES (:AdminId, :Adminname, :Passwordhash)";
            var parameter = new DynamicParameters();
            parameter.Add("AdminId", admin.Adminid);
            parameter.Add("Adminname", admin.Adminname);
            parameter.Add("Passwordhash", admin.Passwordhash);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // تحديث بيانات مشرف (Async)
        public async Task updateAdmin(Admin admin)
        {
            String sql = "UPDATE ADMINS SET Adminname = :Adminname, Passwordhash = :Passwordhash WHERE AdminId = :AdminId";
            var parameter = new DynamicParameters();
            parameter.Add("AdminId", admin.Adminid);
            parameter.Add("Adminname", admin.Adminname);
            parameter.Add("Passwordhash", admin.Passwordhash);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // حذف مشرف (Async)
        public async Task deleteAdmin(int id)
        {
            String sql = "DELETE FROM ADMINS WHERE AdminId = :AdminId";
            var parameter = new DynamicParameters();
            parameter.Add("AdminId", id, DbType.Int32);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // جلب مشرف برقم المعرف (Async)
        public async Task<Admin?> GetAdminId(int id)
        {
            String sql = "SELECT * FROM ADMINS WHERE AdminId = :AdminId";
            var parameter = new DynamicParameters();
            parameter.Add("AdminId", id, DbType.Int32);

            var result = await dbContext.connection1.QueryFirstOrDefaultAsync<Admin>(sql, parameter);
            return result;
        }
        
    }
}