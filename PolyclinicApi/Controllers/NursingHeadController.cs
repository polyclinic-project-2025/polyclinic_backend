using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NursingHeadController : ControllerBase
    {
        private readonly INursingHeadService _nursingHeadService;

        public NursingHeadController(INursingHeadService nursingHeadService)
        {
            _nursingHeadService = nursingHeadService;
        }

        // GET: api/nursinghead/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _nursingHeadService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // GET: api/nursinghead
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _nursingHeadService.GetAllAsync();
            return Ok(result.Value);
        }

        // POST: api/nursinghead
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NursingHeadDto dto)
        {
            var result = await _nursingHeadService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/nursinghead/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NursingHeadDto dto)
        {
            var result = await _nursingHeadService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // DELETE: api/nursinghead/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _nursingHeadService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}
