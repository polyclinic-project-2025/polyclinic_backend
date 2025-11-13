using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicApplication.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // ============================
        // GET: api/departments
        // ============================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            var result = await _departmentService.GetAllAsync();
            return Ok(result);
        }

        // ============================
        // GET: api/departments/{id}
        // ============================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DepartmentDto>> GetById(Guid id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound("Department not found.");

            return Ok(department);
        }

        // ============================
        // POST: api/departments
        // ============================
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var created = await _departmentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================
        // PUT: api/departments/{id}
        // ============================
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            try
            {
                await _departmentService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================
        // DELETE: api/departments/{id}
        // ============================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _departmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
