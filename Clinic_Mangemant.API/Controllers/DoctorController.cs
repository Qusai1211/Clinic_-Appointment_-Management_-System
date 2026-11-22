using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace Clinic_Mangemant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء جلب بيانات الأطباء.", Details = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            try
            {
                var doctorId = await _doctorService.AddDoctorAsync(doctor);
                return Ok(new { Message = "تم إضافة الطبيب بنجاح.", DoctorId = doctorId });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء إضافة الطبيب.", Details = ex.Message });
            } }

            [HttpPut]
            [Route("update")]
            public async Task<IActionResult> UpdateDoctor( Doctor doctor)
            {
                try
                {
                    
                    var result = await _doctorService.UpdateDoctorAsync(doctor);
                    if (!result)
                    {
                        return NotFound(new { Message = "الطبيب غير موجود." });
                    }
                    return Ok(new { Message = "تم تحديث بيانات الطبيب بنجاح." });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء تحديث بيانات الطبيب.", Details = ex.Message });
                }
            }
        [HttpDelete]
        [Route("delete/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            try
            {
                var result = await _doctorService.DeleteDoctorAsync(doctorId);
                if (!result)
                    return NotFound(new { Message = "الطبيب غير موجود." });

                return Ok(new { Message = "تم حذف بيانات الطبيب بنجاح." });
            }
            catch (OracleException ex) when (ex.Number == 20015)
            {
                return Conflict(new { Message = "لا يمكن حذف الطبيب لوجود مواعيد مجدولة له." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "حدث خطأ أثناء حذف بيانات الطبيب.", Details = ex.Message });
            }
        }

        [HttpGet]
            [Route("get/{doctorId}")]
            public async Task<IActionResult> GetDoctor(int doctorId)
            {
                try
                {
                    var doctor = await _doctorService.GetDoctorAsync(doctorId);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "الطبيب غير موجود." });
                    }
                    return Ok(doctor);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء جلب بيانات الطبيب.", Details = ex.Message });
                }
            

        }
    }
}