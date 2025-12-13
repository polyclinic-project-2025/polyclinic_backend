using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
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
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<object>>> Create([FromBody] CreateStockDepartmentDto request)
        {
            var result = await _service.CreateAsync(request);
            if (!result.IsSuccess)
                return BadRequest(ApiResult<object>.BadRequest(result.ErrorMessage!));

            return Ok(ApiResult<object>.Ok(result.Value!, "Stock de departamento creado exitosamente"));
        }

        // GET BY ID
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<object>>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(ApiResult<object>.NotFound(result.ErrorMessage!));

            return Ok(ApiResult<object>.Ok(result.Value!, "Stock obtenido exitosamente"));
        }

        // GET ALL
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<object>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

            return Ok(ApiResult<object>.Ok(result.Value!, "Stocks obtenidos exitosamente"));
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateStockDepartmentDto request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }

            return Ok(ApiResult<bool>.Ok(true, "Stock actualizado exitosamente"));
        }

        // DELETE
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }

            return Ok(ApiResult<bool>.Ok(true, "Stock eliminado exitosamente"));
        }

        // GET STOCK BY DEPARTMENT
        [HttpGet("department/{departmentId:guid}/stock")]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<object>>> GetStockByDepartmentId(Guid departmentId)
        {
            var result = await _service.GetStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return NotFound(ApiResult<object>.NotFound(result.ErrorMessage!));

            return Ok(ApiResult<object>.Ok(result.Value!, "Stock del departamento obtenido"));
        }

        // GET LOW STOCK
        [HttpGet("department/{departmentId:guid}/low-stock")]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        public async Task<ActionResult<ApiResult<object>>> GetLowStockByDepartment(Guid departmentId)
        {
            var result = await _service.GetLowStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return Ok(ApiResult<object>.Ok(new List<object>(), "Sin stocks bajos")); // vacío pero válido

            return Ok(ApiResult<object>.Ok(result.Value!, "Stocks bajos obtenidos"));
        }

        // GET OVER STOCK
        [HttpGet("department/{departmentId:guid}/over-stock")]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        public async Task<ActionResult<ApiResult<object>>> GetOverStockByDepartment(Guid departmentId)
        {
            var result = await _service.GetOverStockByDepartmentIdAsync(departmentId);
            if (!result.IsSuccess)
                return Ok(ApiResult<object>.Ok(new List<object>(), "Sin sobrestocks")); // vacío pero válido

            return Ok(ApiResult<object>.Ok(result.Value!, "Sobrestocks obtenidos"));
        }
    }
}
