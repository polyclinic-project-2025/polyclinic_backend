using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: api/doctor/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _doctorService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // GET: api/doctor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doctorService.GetAllAsync();
            return Ok(result.Value);
        }

        // POST: api/doctor
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorDto dto)
        {
            var result = await _doctorService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/doctor/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DoctorDto dto)
        {
            var result = await _doctorService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // DELETE: api/doctor/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _doctorService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}
