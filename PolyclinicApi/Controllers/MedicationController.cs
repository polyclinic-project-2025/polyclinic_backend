using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationController : ControllerBase
{
    private readonly IMedicationService _service;

    public MedicationController(IMedicationService service)
    {
        _service = service;
    }

    // ============================================================
    // CRUD
    // ============================================================

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicationDto request)
    {
        var result = await _service.CreateAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetById),
            new { id = result.Value!.MedicationId },
            result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDto request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(new { Success = true });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }

    // ============================================================
    // BÚSQUEDAS ESPECIALES
    // ============================================================

    [HttpGet("batch/{batchNumber}")]
    public async Task<IActionResult> GetByBatchNumber(string batchNumber)
    {
        var result = await _service.GetByBatchNumberAsync(batchNumber);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("company/{company}")]
    public async Task<IActionResult> GetByCompany(string company)
    {
        var result = await _service.GetByCommercialCompanyAsync(company);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchByName(string name)
    {
        var result = await _service.SearchByNameAsync(name);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ============================================================
    // STOCK — MÉTODOS ESPECIALES
    // ============================================================

    [HttpGet("stock/low/warehouse")]
    public async Task<IActionResult> GetLowWarehouseStock()
    {
        var result = await _service.GetLowStockWarehouseAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("stock/low/nurse")]
    public async Task<IActionResult> GetLowNurseStock()
    {
        var result = await _service.GetLowStockNurseAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("stock/over/warehouse")]
    public async Task<IActionResult> GetOverWarehouseStock()
    {
        var result = await _service.GetOverStockWarehouseAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("stock/over/nurse")]
    public async Task<IActionResult> GetOverNurseStock()
    {
        var result = await _service.GetOverStockNurseAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}