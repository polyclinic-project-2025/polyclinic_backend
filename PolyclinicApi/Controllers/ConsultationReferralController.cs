using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicApplication.Services.Interfaces;
using System.Text.Json;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response.Export;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationReferralController : ControllerBase
{
    private readonly IConsultationReferralService _consultationReferralService;
    private readonly IExportService _exportService;

    public ConsultationReferralController(IConsultationReferralService consultationReferralService, IExportService exportService)
    {
        _consultationReferralService = consultationReferralService;
        _exportService = exportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConsultationReferralResponse>>> GetAll()
    {
        var result = await _consultationReferralService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationReferralResponse>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<ConsultationReferralResponse>>.Ok(result.Value!, "Consultas obtenidas exitosamente"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConsultationReferralResponse>> GetById(Guid id)
    {
        var result = await _consultationReferralService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<ConsultationReferralResponse>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<ConsultationReferralResponse>.Ok(result.Value!, "Consulta obtenida exitosamente"));
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<ConsultationReferralResponse>>> GetLastTen()
    {
        var result = await _consultationReferralService.GetLastTen();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationReferralResponse>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<ConsultationReferralResponse>>.Ok(result.Value!, "Últimas consultas obtenidas"));
    }

    [HttpGet("{in-range}")]
    public async Task<ActionResult<IEnumerable<ConsultationReferralResponse>>> GetInRange([FromQuery] DateTime start,
    [FromQuery] DateTime end)
    {
        var result = await _consultationReferralService.GetConsultationInRange(start, end);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<ConsultationReferralResponse>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<ConsultationReferralResponse>>.Ok(result.Value!, "Consultas en rango obtenidas"));
    } 

    [HttpPost]
    public async Task<ActionResult<ConsultationReferralResponse>> Create([FromBody] CreateConsultationReferralDto request)
    {
        var result = await _consultationReferralService.CreateAsync(request);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<ConsultationReferralResponse>.BadRequest(result.ErrorMessage!));
        
        return Ok(ApiResult<ConsultationReferralResponse>.Ok(result.Value!, "Consulta creada exitosamente"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ConsultationReferralResponse>> Update(Guid id, [FromBody] UpdateConsultationReferralDto request)
    {
        var result = await _consultationReferralService.UpdateAsync(id, request);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<ConsultationReferralResponse>.BadRequest(result.ErrorMessage!));
        
        return Ok(ApiResult<ConsultationReferralResponse>.Ok(result.Value!, "Consulta actualizada exitosamente"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _consultationReferralService.DeleteAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
        
        return Ok(ApiResult<bool>.Ok(true, "Consulta eliminada exitosamente"));
    }

    // ============================
    // GET: api/consultationreferral/export
    // Exportar todas las consultas de referencia a PDF
    // ============================
    [HttpGet("export")]
    public async Task<ActionResult> ExportConsultationReferrals()
    {
        var consultationsResult = await _consultationReferralService.GetAllAsync();
        if (!consultationsResult.IsSuccess)
            return BadRequest(ApiResult<ExportResponse>.Error(consultationsResult.ErrorMessage!));

        string jsonData = JsonSerializer.Serialize(consultationsResult.Value);
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"consultation_referrals_{Guid.NewGuid()}.pdf");

        var exportResult = await _exportService.ExportDataAsync(jsonData, "pdf", tempFilePath);
        if (!exportResult.IsSuccess)
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));

        return Ok(ApiResult<ExportResponse>.Ok(exportResult.Value!, "Consultas exportadas exitosamente"));
    }
}