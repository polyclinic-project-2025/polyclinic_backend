using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalStaffController : ControllerBase
    {
        private readonly IMedicalStaffService _medicalStaffService;

        public MedicalStaffController(IMedicalStaffService medicalStaffService)
        {
            _medicalStaffService = medicalStaffService;
        }

        // GET: api/medicalstaff/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _medicalStaffService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // GET: api/medicalstaff
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _medicalStaffService.GetAllAsync();
            return Ok(result.Value);
        }

        // POST: api/medicalstaff
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicalStaffDto dto)
        {
            var result = await _medicalStaffService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.DepartmentId }, result.Value);
        }

        // PUT: api/medicalstaff/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MedicalStaffDto dto)
        {
            var result = await _medicalStaffService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // DELETE: api/medicalstaff/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _medicalStaffService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}
