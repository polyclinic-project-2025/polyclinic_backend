using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolyclinicAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IExportService _exportService;

        public DepartmentsController(IDepartmentService departmentService, IExportService exportService)
        {
            _departmentService = departmentService;
            _exportService = exportService;
        }

        /// <summary>
        /// Obtiene todos los departamentos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<DepartmentDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<IEnumerable<DepartmentDto>>>> GetAll()
        {
            var result = await _departmentService.GetAllAsync();
            
            if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<DepartmentDto>>.Error(result.ErrorMessage!));

            return Ok(ApiResult<IEnumerable<DepartmentDto>>.Ok(result.Value!, "Departamentos obtenidos exitosamente"));
        }

        /// <summary>
        /// Obtiene un departamento por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<DepartmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<DepartmentDto>>> GetById(Guid id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            
            if (!result.IsSuccess)
                return NotFound(ApiResult<DepartmentDto>.NotFound(result.ErrorMessage!));

            return Ok(ApiResult<DepartmentDto>.Ok(result.Value!, "Departamento obtenido exitosamente"));
        }

        /// <summary>
        /// Crea un nuevo departamento
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<DepartmentDto>), 201)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<DepartmentDto>>> Create([FromBody] CreateDepartmentDto dto)
        {
            var result = await _departmentService.CreateAsync(dto);
            
            if (!result.IsSuccess)
                return BadRequest(ApiResult<DepartmentDto>.BadRequest(result.ErrorMessage!));

            var apiResult = ApiResult<DepartmentDto>.Ok(result.Value!, "Departamento creado exitosamente");
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.DepartmentId }, apiResult);
        }

        /// <summary>
        /// Actualiza un departamento existente
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            var result = await _departmentService.UpdateAsync(id, dto);
            
            if (!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }

            return Ok(ApiResult<bool>.Ok(true, "Departamento actualizado exitosamente"));
        }

        /// <summary>
        /// Elimina un departamento
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
        {
            var result = await _departmentService.DeleteAsync(id);
            
            if (!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }

            return Ok(ApiResult<bool>.Ok(true, "Departamento eliminado exitosamente"));
        }

        /// <summary>
        /// Obtiene los doctores de un departamento específico
        /// </summary>
        [HttpGet("{id:guid}/doctors")]
        [ProducesResponseType(typeof(ApiResult<List<DoctorResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<List<DoctorResponse>>>> GetDoctorsByDepartment(Guid id)
        {
            var result = await _departmentService.GetDoctorsByDepartmentIdAsync(id);
            
            if (!result.IsSuccess)
                return BadRequest(ApiResult<List<DoctorResponse>>.Error(result.ErrorMessage!));

            return Ok(ApiResult<List<DoctorResponse>>.Ok(result.Value!, "Doctores obtenidos exitosamente"));
        }

        /// <summary>
        /// Exporta todos los departamentos a PDF
        /// </summary>
        [HttpGet("export")]
        [ProducesResponseType(typeof(ApiResult<ExportResponse>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<ExportResponse>>> ExportDepartments()
        {
            // Obtener todos los departamentos
            var departmentsResult = await _departmentService.GetAllAsync();
            
            if (!departmentsResult.IsSuccess)
                return BadRequest(ApiResult<ExportResponse>.Error(departmentsResult.ErrorMessage!));

            // Serializar a JSON
            string jsonData = JsonSerializer.Serialize(departmentsResult.Value);

            // Generar archivo temporal
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"departments_{Guid.NewGuid()}.pdf");

            // Exportar usando el servicio
            var exportResult = await _exportService.ExportDataAsync(jsonData, "pdf", tempFilePath);
            
            if (!exportResult.IsSuccess)
            {
                return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
            }

            return Ok(ApiResult<ExportResponse>.Ok(exportResult.Value!, "Departamentos exportados exitosamente"));
        }
    }
}
