using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialty == null)
            {
                return NotFound(new { Message = $"Specialty with ID {id} not found." });
            }
            return Ok(specialty);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialtyUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SpecialtyName))
            {
                return BadRequest(new { Message = "Specialty name is required." });
            }

            var id = await _specialtyService.CreateSpecialtyAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { SpecialtyId = id, Message = "Specialty created successfully." });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, [FromBody] SpecialtyUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SpecialtyName))
            {
                return BadRequest(new { Message = "Specialty name is required." });
            }

            var updated = await _specialtyService.UpdateSpecialtyAsync(id, dto);
            if (!updated)
            {
                return NotFound(new { Message = $"Specialty with ID {id} not found." });
            }

            return Ok(new { Message = "Specialty updated successfully." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var deleted = await _specialtyService.DeleteSpecialtyAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Specialty with ID {id} not found." });
            }

            return Ok(new { Message = "Specialty deleted successfully." });
        }
    }
}
