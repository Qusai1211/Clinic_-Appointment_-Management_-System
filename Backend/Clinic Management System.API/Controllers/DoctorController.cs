using Clinic_Management_System.infra.Service;
using Clininc_Management_System.Core.Models;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
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

        // GET: api/Doctor
        [HttpGet]
        public async Task<ActionResult<List<Doctor>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetDoctorById(id);
            if (doctor == null)
            {
                return NotFound(new { message = "الطبيب غير موجود" });
            }
            return Ok(doctor);
        }

        // POST: api/Doctor
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] Doctor doctor)
        {
            if (doctor == null)
            {
                return BadRequest("بيانات الطبيب غير صالحة.");
            }

            await _doctorService.CreateDoctor(doctor);
            return Ok(new { message = "تم إضافة الطبيب بنجاح" });
        }

        // PUT: api/Doctor/5
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return BadRequest("معرف الطبيب غير متطابق.");
            }

            await _doctorService.UpdateDoctor(doctor);
            return Ok(new { message = "تم تحديث بيانات الطبيب بنجاح" });
        }

        // DELETE: api/Doctor/5
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var isDeleted = await _doctorService.DeleteDoctor(id);
            if (!isDeleted) //   أي لم يتم العثور على الطبيب أو فشل الحذف false
            {
                return NotFound(new { message = "الطبيب المراد حذفه غير موجود" });
            }

            return Ok(new { message = "تم حذف الطبيب بنجاح" });
        }
    }
}
