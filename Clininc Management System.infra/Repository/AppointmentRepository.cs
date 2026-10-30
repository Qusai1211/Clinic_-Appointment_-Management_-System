using Clininc_Management_System.Core.common;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Repository;
using Clininc_Management_System.infra.Common;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Repository
{
    public class AppointmentRepository: IAppointmentRepository
    {
        private readonly IDbContext dbcontext;

        public AppointmentRepository(IDbContext dbcontext1)
        {
            dbcontext = dbcontext1;
        }

        // جلب كل المواعيد (Async)
        public async Task<List<Appointment>> GetAppointments()
        {
            String sql1 = "SELECT * FROM Appointments";
            var result = await dbcontext.connection1.QueryAsync<Appointment>(sql1);
            return result.ToList();
        }

        // إضافة موعد جديد (Async)
        public async Task createAppointment(Appointment appointment)
        {
            String sql = "INSERT INTO Appointments(APPOINTMENTID, REFERENCENUMBER, APPOINTMENTDATE, APPOINTMENTTIME, STATUS, DOCTORID, SLOTID, PATIENTID) " +
                         "VALUES (:APPOINTMENTID, :REFERENCENUMBER, :APPOINTMENTDATE, :APPOINTMENTTIME, :STATUS, :DOCTORID, :SLOTID, :PATIENTID)";

            var parameter = new DynamicParameters();
            parameter.Add("APPOINTMENTID", appointment.Appointmentid);
            parameter.Add("REFERENCENUMBER", appointment.Referencenumber);
            parameter.Add("APPOINTMENTDATE", appointment.Appointmentdate);
            parameter.Add("APPOINTMENTTIME", appointment.Appointmenttime);
            parameter.Add("STATUS", appointment.Status);
            parameter.Add("DOCTORID", appointment.Doctorid);
            parameter.Add("SLOTID", appointment.Slotid);
            parameter.Add("PATIENTID", appointment.Patientid);

            await dbcontext.connection1.ExecuteAsync(sql, parameter);
        }

        // تعديل موعد (Async)
        public async Task updateAppointment(Appointment appointment)
        {
            String sql = "UPDATE Appointments SET REFERENCENUMBER = :REFERENCENUMBER, APPOINTMENTDATE = :APPOINTMENTDATE, " +
                         "APPOINTMENTTIME = :APPOINTMENTTIME, STATUS = :STATUS, DOCTORID = :DOCTORID, " +
                         "SLOTID = :SLOTID, PATIENTID = :PATIENTID WHERE APPOINTMENTID = :APPOINTMENTID";

            var parameter = new DynamicParameters();
            parameter.Add("APPOINTMENTID", appointment.Appointmentid);
            parameter.Add("REFERENCENUMBER", appointment.Referencenumber);
            parameter.Add("APPOINTMENTDATE", appointment.Appointmentdate);
            parameter.Add("APPOINTMENTTIME", appointment.Appointmenttime);
            parameter.Add("STATUS", appointment.Status);
            parameter.Add("DOCTORID", appointment.Doctorid);
            parameter.Add("SLOTID", appointment.Slotid);
            parameter.Add("PATIENTID", appointment.Patientid);

            await dbcontext.connection1.ExecuteAsync(sql, parameter);
        }

        // حذف موعد (Async)
        public async Task deleteAppointment(int id)
        {
            String sql = "DELETE FROM Appointments WHERE APPOINTMENTID = :APPOINTMENTID";
            var parameter = new DynamicParameters();
            parameter.Add("APPOINTMENTID", id, DbType.Int32);

            await dbcontext.connection1.ExecuteAsync(sql, parameter);
        }

        // جلب موعد معين برقم المعرف (Async)
        public async Task<Appointment?> GetAppointment(int id)
        {
            String sql = "SELECT * FROM Appointments WHERE APPOINTMENTID = :APPOINTMENTID";
            var parameter = new DynamicParameters();
            parameter.Add("APPOINTMENTID", id, DbType.Int32);

            var result = await dbcontext.connection1.QueryFirstOrDefaultAsync<Appointment>(sql, parameter);
            return result;
        }
    }
}