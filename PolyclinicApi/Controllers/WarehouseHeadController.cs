using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseManagerController : ControllerBase
    {
        private readonly IWarehouseManagerService _warehouseManagerService;

        public WarehouseManagerController(IWarehouseManagerService warehouseManagerService)
        {
            _warehouseManagerService = warehouseManagerService;
        }

        // GET: api/warehousemanager/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _warehouseManagerService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // GET: api/warehousemanager
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _warehouseManagerService.GetAllAsync();
            return Ok(result.Value);
        }

        // POST: api/warehousemanager
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehouseManagerDto dto)
        {
            var result = await _warehouseManagerService.CreateAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        // PUT: api/warehousemanager/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] WarehouseManagerDto dto)
        {
            var result = await _warehouseManagerService.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        // DELETE: api/warehousemanager/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _warehouseManagerService.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}
