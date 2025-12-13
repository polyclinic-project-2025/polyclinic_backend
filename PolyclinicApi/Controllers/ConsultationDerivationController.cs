using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationDerivationController : ControllerBase
{
    private readonly IConsultationDerivationService _service;

    public ConsultationDerivationController(IConsultationDerivationService service)
    {
        _service = service;
    }

    // ************************************************************
    // POST - CREATE
    // ************************************************************
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConsultationDerivationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<ConsultationDerivationDto>.BadRequest(result.ErrorMessage!));

        return Ok(ApiResult<ConsultationDerivationDto>.Ok(result.Value!, "Consulta creada exitosamente"));
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConsultationDerivationDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));

        return Ok(ApiResult<bool>.Ok(true, "Consulta actualizada exitosamente"));
    }

    // ************************************************************
    // DELETE
    // ************************************************************
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));

        return Ok(ApiResult<bool>.Ok(true, "Consulta eliminada exitosamente"));
    }

    // ************************************************************
    // GET BY ID
    // ************************************************************
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<ConsultationDerivationDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<ConsultationDerivationDto>.Ok(result.Value!, "Consulta obtenida exitosamente"));
    }

    // ************************************************************
    // GET ALL
    // ************************************************************
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationDerivationDto>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<ConsultationDerivationDto>>.Ok(result.Value!, "Consultas obtenidas exitosamente"));
    }

    // ************************************************************
    // CUSTOM: GET BY DATE RANGE
    // ************************************************************
    [HttpGet("date-range")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] Guid patientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationDerivationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<ConsultationDerivationDto>>.Ok(result.Value!, "Consultas en rango obtenidas"));
    }

    // ************************************************************
    // CUSTOM: LAST 10
    // ************************************************************
    [HttpGet("last-10/{patientId:guid}")]
    public async Task<IActionResult> GetLast10(Guid patientId)
    {
        var result = await _service.GetLast10ByPatientIdAsync(patientId);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationDerivationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<ConsultationDerivationDto>>.Ok(result.Value!, "Últimas consultas obtenidas"));
    }
}