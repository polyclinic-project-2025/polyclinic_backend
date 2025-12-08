using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.StockDepartment;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.Services.Interfaces;


namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockDepartmentController : ControllerBase
    {
        private readonly IStockDepartmentService _service;

        public StockDepartmentController(IStockDepartmentService service)
        {
            _service = service;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockDepartmentDto request)
        {
            var result = await _service.CreateAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Value);
        }

        // GET BY ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Value);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Value);
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStockDepartmentDto request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return NoContent(); // update success
        }

        // DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }

        // GET STOCK BY DEPARTMENT
        [HttpGet("department/{departmentId:guid}/stock")]
        public async Task<IActionResult> GetStockByDepartmentId(Guid departmentId)
        {
            var result = await _service.GetStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Value);
        }

        // GET LOW STOCK
        [HttpGet("department/{departmentId:guid}/low-stock")]
        public async Task<IActionResult> GetLowStockByDepartment(Guid departmentId)
        {
            var result = await _service.GetLowStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return Ok(new List<object>()); // vacío pero válido

            return Ok(result.Value);
        }

        // GET OVER STOCK
        [HttpGet("department/{departmentId:guid}/over-stock")]
        public async Task<IActionResult> GetOverStockByDepartment(Guid departmentId)
        {
            var result = await _service.GetOverStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return Ok(new List<object>()); // vacío pero válido

            return Ok(result.Value);
        }
    }
}
