using Clininc_Management_System.Core.DTO;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var patients = await _patientService.GetPatientsAsync(search);
            return Ok(patients);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound(new { Message = $"Patient with ID {id} not found." });
            }
            return Ok(patient);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPatientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest(new { Message = "Full name is required." });
            }
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new { Message = "Phone number is required." });
            }
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { Message = "Email address is required." });
            }

            try
            {
                var patientId = await _patientService.RegisterPatientAsync(request);
                var patient = await _patientService.GetPatientByIdAsync(patientId);

                return Ok(new
                {
                    Success = true,
                    Message = "Patient registered successfully.",
                    Patient = patient
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(decimal id, [FromBody] PatientUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return BadRequest(new { Message = "Full name is required." });
            }
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return BadRequest(new { Message = "Phone number is required." });
            }

            try
            {
                var updated = await _patientService.UpdatePatientAsync(id, dto);
                if (!updated)
                {
                    return NotFound(new { Message = $"Patient with ID {id} not found." });
                }

                return Ok(new { Success = true, Message = "Patient record updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            try
            {
                var deleted = await _patientService.DeletePatientAsync(id);
                if (!deleted)
                {
                    return NotFound(new { Message = $"Patient with ID {id} not found." });
                }

                return Ok(new { Success = true, Message = "Patient record deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}