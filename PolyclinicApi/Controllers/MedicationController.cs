using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request.Export;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationController : ControllerBase
{
    private readonly IMedicationService _service;
    private readonly IExportService _exportService;

    public MedicationController(IMedicationService service, IExportService exportService)
    {
        _service = service;
        _exportService = exportService;
    }

    // ============================================================
    // CRUD
    // ============================================================

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<MedicationDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<MedicationDto>>> Create([FromBody] CreateMedicationDto request)
    {
        var result = await _service.CreateAsync(request);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<MedicationDto>.BadRequest(result.ErrorMessage!));

        var apiResult = ApiResult<MedicationDto>.Ok(result.Value!, "Medicamento creado exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.MedicationId }, apiResult);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<MedicationDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<MedicationDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(ApiResult<MedicationDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<MedicationDto>.Ok(result.Value!, "Medicamento obtenido exitosamente"));
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos obtenidos exitosamente"));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateMedicationDto request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }

        return Ok(ApiResult<bool>.Ok(true, "Medicamento actualizado exitosamente"));
    }

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

        return Ok(ApiResult<bool>.Ok(true, "Medicamento eliminado exitosamente"));
    }

    // ============================================================
    // BÚSQUEDAS ESPECIALES
    // ============================================================

    [HttpGet("batch/{batchNumber}")]
    [ProducesResponseType(typeof(ApiResult<MedicationDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<MedicationDto>>> GetByBatchNumber(string batchNumber)
    {
        var result = await _service.GetByBatchNumberAsync(batchNumber);

        if (!result.IsSuccess)
            return NotFound(ApiResult<MedicationDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<MedicationDto>.Ok(result.Value!, "Medicamento encontrado"));
    }

    [HttpGet("company/{company}")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetByCompany(string company)
    {
        var result = await _service.GetByCommercialCompanyAsync(company);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos encontrados"));
    }

    [HttpGet("search/{name}")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> SearchByName(string name)
    {
        var result = await _service.SearchByNameAsync(name);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos encontrados"));
    }

    // ============================================================
    // STOCK — MÉTODOS ESPECIALES
    // ============================================================

    [HttpGet("stock/low/warehouse")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetLowWarehouseStock()
    {
        var result = await _service.GetLowStockWarehouseAsync();

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos con bajo stock obtenidos"));
    }

    [HttpGet("stock/low/nurse")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetLowNurseStock()
    {
        var result = await _service.GetLowStockNurseAsync();

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos de enfermería con bajo stock obtenidos"));
    }

    [HttpGet("stock/over/warehouse")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetOverWarehouseStock()
    {
        var result = await _service.GetOverStockWarehouseAsync();

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos con sobrestock obtenidos"));
    }

    [HttpGet("stock/over/nurse")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationDto>>>> GetOverNurseStock()
    {
        var result = await _service.GetOverStockNurseAsync();

        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<MedicationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationDto>>.Ok(result.Value!, "Medicamentos de enfermería con sobrestock obtenidos"));
    }

    // ============================
    // GET: api/medication/export
    // Exportar medicamentos a PDF con columnas seleccionadas
    // ============================
    [HttpGet("export")]
    [ProducesResponseType(typeof(ApiResult<ExportResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<ExportResponse>>> ExportMedications(
        [FromQuery] string format = "pdf", 
        [FromQuery] List<string>? columns = null,
        [FromQuery] string name = "Medicamentos")
    {
        // Obtener todos los medicamentos
        var medicationsResult = await _service.GetAllAsync();
        if (!medicationsResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(medicationsResult.ErrorMessage!));
        }

        // Crear el DTO de exportación
        var exportDto = new ExportDto
        {
            Format = format,
            Fields = columns ?? new List<string> {"CommercialName", "BatchNumber", "Format", "ExpirationDate", "QuantityWarehouse", "QuantityNurse"},
            Data = medicationsResult.Value!,
            Name = name
        };

        // Exportar usando el servicio
        var exportResult = await _exportService.ExportDataAsync(exportDto);
        
        if (!exportResult.IsSuccess)
        {
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
        }

        return Ok(ApiResult<ExportResponse>.Ok(exportResult.Value!, "Medicamentos exportados exitosamente"));
    }
}