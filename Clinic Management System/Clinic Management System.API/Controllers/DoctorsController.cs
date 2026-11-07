using Clininc_Management_System.Core.DTOs;
using Clininc_Management_System.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Management_System.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] decimal? specialtyId, [FromQuery] string? sortBy)
        {
            var doctors = await _doctorService.GetDoctorsAsync(search, specialtyId, sortBy);
            return Ok(doctors);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var featured = await _doctorService.GetFeaturedDoctorsAsync();
            return Ok(featured);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            var doctor = await _doctorService.GetDoctorDetailsAsync(id);
            if (doctor == null)
            {
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            }
            return Ok(doctor);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return BadRequest(new { Message = "Doctor full name is required." });
            }

            var id = await _doctorService.CreateDoctorAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { DoctorId = id, Message = "Doctor created successfully." });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, [FromBody] DoctorUpsertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return BadRequest(new { Message = "Doctor full name is required." });
            }

            var updated = await _doctorService.UpdateDoctorAsync(id, dto);
            if (!updated)
            {
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            }

            return Ok(new { Message = "Doctor updated successfully." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var deleted = await _doctorService.DeleteDoctorAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"Doctor with ID {id} not found." });
            }

            return Ok(new { Message = "Doctor deleted successfully." });
        }

        [HttpGet("{id}/working-hours")]
        public async Task<IActionResult> GetWorkingHours(decimal id)
        {
            var hours = await _doctorService.GetWorkingHoursAsync(id);
            return Ok(hours);
        }

        [Authorize]
        [HttpPost("{id}/working-hours")]
        public async Task<IActionResult> AddWorkingHours(decimal id, [FromBody] WorkingHoursUpsertDto dto)
        {
            dto.DoctorId = id;
            var whId = await _doctorService.AddWorkingHoursAsync(dto);
            return Ok(new { WorkingHoursId = whId, Message = "Working hours added successfully." });
        }

        [Authorize]
        [HttpDelete("working-hours/{workingHoursId}")]
        public async Task<IActionResult> DeleteWorkingHours(decimal workingHoursId)
        {
            var deleted = await _doctorService.DeleteWorkingHoursAsync(workingHoursId);
            if (!deleted)
            {
                return NotFound(new { Message = $"Working hours record {workingHoursId} not found." });
            }
            return Ok(new { Message = "Working hours deleted successfully." });
        }

        [Authorize]
        [HttpPut("working-hours/{workingHoursId}")]
        public async Task<IActionResult> UpdateWorkingHours(decimal workingHoursId, [FromBody] WorkingHoursUpsertDto dto)
        {
            var updated = await _doctorService.UpdateWorkingHoursAsync(workingHoursId, dto);
            if (!updated)
            {
                return NotFound(new { Message = $"Working hours record {workingHoursId} not found." });
            }
            return Ok(new { Message = "Working hours updated successfully." });
        }

        [HttpGet("{id}/slots")]
        public async Task<IActionResult> GetSlots(decimal id, [FromQuery] DateTime? date, [FromQuery] bool availableOnly = false)
        {
            var slots = await _doctorService.GetDoctorSlotsAsync(id, date, availableOnly);
            return Ok(slots);
        }

        [Authorize]
        [HttpPost("{id}/slots")]
        public async Task<IActionResult> AddSlot(decimal id, [FromBody] SlotCreateDto dto)
        {
            dto.DoctorId = id;
            var slotId = await _doctorService.AddSlotAsync(dto);
            return Ok(new { SlotId = slotId, Message = "Appointment slot added successfully." });
        }

        [Authorize]
        [HttpDelete("slots/{slotId}")]
        public async Task<IActionResult> DeleteSlot(decimal slotId)
        {
            var deleted = await _doctorService.DeleteSlotAsync(slotId);
            if (!deleted)
            {
                return NotFound(new { Message = $"Slot {slotId} not found." });
            }
            return Ok(new { Message = "Slot deleted successfully." });
        }

        [Authorize]
        [HttpPut("slots/{slotId}/availability")]
        public async Task<IActionResult> UpdateSlotAvailability(decimal slotId, [FromBody] SlotUpdateDto dto)
        {
            var updated = await _doctorService.UpdateSlotStatusAsync(slotId, dto.IsAvailable);
            if (!updated)
            {
                return NotFound(new { Message = $"Slot {slotId} not found." });
            }
            return Ok(new { Message = $"Slot availability updated to {dto.IsAvailable} successfully." });
        }
    }
}

