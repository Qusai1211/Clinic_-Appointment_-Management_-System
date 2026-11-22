using Clinic_Managemeant.Core.Models;
using Clinic_Managemeant.Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Mangemant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        [HttpPost]
        [Route("AddPatient")]
        public async Task<IActionResult> AddPatient(Patient patient)
        {
            try
            {
                var result = await _patientService.AddPatientAsync(patient);
                return Ok(new { Message = "تم إضافة بيانات المريض بنجاح.", PatientId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء إضافة بيانات المريض.", Details = ex.Message });
            }

        }
        [HttpPut]
        [Route("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient(Patient patient)
        {
            try
            {
                var result = await _patientService.UpdatePatientAsync(patient);
                if (result)
                {
                    return Ok(new { Message = "تم تحديث بيانات المريض بنجاح." });
                }
                else
                {
                    return NotFound(new { Message = "المريض غير موجود." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء تحديث بيانات المريض.", Details = ex.Message });
            }
           
        }
        [HttpDelete]
        [Route("DeletePatient/{patientId}")]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(patientId);
                if (result)
                {
                    return Ok(new { Message = "تم حذف بيانات المريض بنجاح." });
                }
                else
                {
                    return NotFound(new { Message = "المريض غير موجود." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء حذف بيانات المريض.", Details = ex.Message });
            }
           
            }
        [HttpGet]
        [Route("GetPatientById/{patientId}")]
        public async Task<IActionResult> GetPatientById(int patientId)
        {
            try
            {
                var result = await _patientService.GetPatientByIdAsync(patientId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(new { Message = "المريض غير موجود." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء جلب بيانات المريض.", Details = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetAllPatients")]
        public async Task<IActionResult> GetAllPatients()
        {
            try
            {
                var result = await _patientService.GetAllPatientsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "حدث خطأ أثناء جلب بيانات المرضى.", Details = ex.Message });
            }
        }

        }
}

