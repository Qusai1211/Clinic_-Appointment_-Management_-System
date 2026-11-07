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
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IDbContext dbContext;

        public DoctorRepository(IDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        // جلب كل الأطباء (Async)
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            String sql = "SELECT * FROM DOCTORS";
            var result = await dbContext.connection1.QueryAsync<Doctor>(sql);
            return result.ToList();
        }

        // إضافة طبيب جديد (Async)
        public async Task CreateDoctor(Doctor doctor)
        {
            String sql = "INSERT INTO DOCTORS (DOCTORID, FULLNAME, QUALIFICATIONS, CONSULTATIONFEE, PROFILEIMAGEURL, ISFEATURED, SPECIALTYID) VALUES (:DoctorId, :FullName, :Qualifications, :ConsultationFee, :ProfileImageUrl, :IsFeatured, :SpecialtyId)";
            var parameter = new DynamicParameters();
            parameter.Add("DoctorId", doctor.DoctorId);
            parameter.Add("FullName", doctor.Fullname);
            parameter.Add("Qualifications", doctor.Qualifications);
            parameter.Add("ConsultationFee", doctor.Consultationfee);
            parameter.Add("ProfileImageUrl", doctor.Profileimageurl);
            parameter.Add("IsFeatured", doctor.Isfeatured);
            parameter.Add("SpecialtyId", doctor.Specialtyid);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // تحديث بيانات طبيب (Async)
        public async Task UpdateDoctor(Doctor doctor)
        {
            String sql = "UPDATE DOCTORS SET FULLNAME = :FullName, QUALIFICATIONS = :Qualifications, CONSULTATIONFEE = :ConsultationFee, PROFILEIMAGEURL = :ProfileImageUrl, ISFEATURED = :IsFeatured, SPECIALTYID = :SpecialtyId WHERE DOCTORID = :DoctorId";
            var parameter = new DynamicParameters();
            parameter.Add("DoctorId", doctor.DoctorId);
            parameter.Add("FullName", doctor.Fullname);
            parameter.Add("Qualifications", doctor.Qualifications);
            parameter.Add("ConsultationFee", doctor.Consultationfee);
            parameter.Add("ProfileImageUrl", doctor.Profileimageurl);
            parameter.Add("IsFeatured", doctor.Isfeatured);
            parameter.Add("SpecialtyId", doctor.Specialtyid);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // حذف طبيب (Async)
        public async Task DeleteDoctor(int id)
        {
            String sql = "DELETE FROM DOCTORS WHERE DOCTORID = :DoctorId";
            var parameter = new DynamicParameters();
            parameter.Add("DoctorId", id, DbType.Int32);

            await dbContext.connection1.ExecuteAsync(sql, parameter);
        }

        // جلب طبيب برقم المعرف (Async)
        public async Task<Doctor?> GetDoctorById(int id)
        {
            String sql = "SELECT * FROM DOCTORS WHERE DOCTORID = :DoctorId";
            var parameter = new DynamicParameters();
            parameter.Add("DoctorId", id, DbType.Int32);

            var result = await dbContext.connection1.QueryFirstOrDefaultAsync<Doctor>(sql, parameter);
            return result;
        }


    }
}