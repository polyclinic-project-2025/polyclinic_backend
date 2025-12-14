using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Services.Interfaces.Analytics;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IUnifiedConsultationService _service;
    private readonly IMedicationConsumptionService _medicationConsumptionService;

    public AnalyticsController(
        IUnifiedConsultationService service,
        IMedicationConsumptionService medicationConsumptionService)
    {
        _service = service;
        _medicationConsumptionService = medicationConsumptionService;
    }

    // GET: api/Analytics/last10/{patientId}
    [HttpGet("last10/{patientId:guid}")]
    public async Task<IActionResult> GetLast10(Guid patientId)
    {
        var result = await _service.GetLast10ByPatientIdAsync(patientId);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<UnifiedConsultationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<UnifiedConsultationDto>>.Ok(result.Value!, "Últimas consultas obtenidas"));
    }

    // GET: api/Analytics/range?patientId=...&startDate=...&endDate=...
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange(
        [FromQuery] Guid patientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<UnifiedConsultationDto>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<UnifiedConsultationDto>>.Ok(result.Value!, "Consultas en rango obtenidas"));
    }

    // GET: api/Analytics/medication-consumption?medicationId=...&month=...&year=...
    [HttpGet("medication-consumption")]
    public async Task<IActionResult> GetMedicationConsumption(
        [FromQuery] Guid medicationId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var result = await _medicationConsumptionService.GetMonthlyConsumptionAsync(medicationId, month, year);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResult<MedicationConsumptionReadModel>.Error(result.ErrorMessage!));

        return Ok(ApiResult<MedicationConsumptionReadModel>.Ok(result.Value!, "Consumo de medicamento obtenido"));
    }
}