using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestoExternoController : ControllerBase
    {
        private readonly IPuestoExternoService _peService;

        public PuestoExternoController(IPuestoExternoService peService)
        {
            _peService = peService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PuestoExternoDto>>> GetAll()
        {
            var result = await _peService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PuestoExternoDto>> GetById(Guid id)
        {
            var pe = await _peService.GetByIdAsync(id);
            if (pe == null)
                return NotFound("ExternalMedicalPost not found.");

            return Ok(pe);
        }

        [HttpPost]
        public async Task<ActionResult<PuestoExternoDto>> Create([FromBody] CreatePuestoExternoDto dto)
        {
            try
            {
                var created = await _peService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.PuestoExternoId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _peService.DeleteAsync(id);
            return NoContent();
        }
    }
}