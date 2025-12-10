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
public class AnalyticsController : ControllerBase
{
    private readonly IUnifiedConsultationService _service;

    public AnalyticsController(IUnifiedConsultationService service)
    {
        _service = service;
    }

    // GET: api/UnifiedConsultation/last10/{patientId}
    [HttpGet("last10/{patientId:guid}")]
    public async Task<IActionResult> GetLast10(Guid patientId)
    {
        var result = await _service.GetLast10ByPatientIdAsync(patientId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // GET: api/UnifiedConsultation/range?patientId=...&startDate=...&endDate=...
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange(
        [FromQuery] Guid patientId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}
