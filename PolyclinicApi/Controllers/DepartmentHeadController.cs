using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs.Request;

namespace PolyclinicAPI.Controllers
{
    /// <summary>
    /// Gestión de jefes de departamento
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentHeadController : ControllerBase
    {
        private readonly IDepartmentHeadService _departmentHeadService;

        public DepartmentHeadController(IDepartmentHeadService departmentHeadService)
        {
            _departmentHeadService = departmentHeadService;
        }

        /// <summary>
        /// Obtiene un jefe de departamento por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _departmentHeadService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        /// <summary>
        /// Lista todos los jefes de departamento
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentHeadService.GetAllAsync();
            return Ok(result.Value);
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] DepartmentHeadDto dto)
        {
            var result = await _departmentHeadService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        /// <summary>
        /// Actualiza un jefe de departamento
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] DepartmentHeadDto dto)
        {
            var result = await _departmentHeadService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        /// <summary>
        /// Elimina un jefe de departamento
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _departmentHeadService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}