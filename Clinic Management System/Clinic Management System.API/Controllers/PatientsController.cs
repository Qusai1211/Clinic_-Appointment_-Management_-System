using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Service;
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

        [HttpGet("{id}")]
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
    }
}
