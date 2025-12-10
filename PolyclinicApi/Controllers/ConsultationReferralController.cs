using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicApplication.Services.Interfaces;
using System.Text.Json;

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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConsultationReferralResponse>> GetById(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("recent")]

    public async Task<ActionResult<IEnumerable<ConsultationReferralResponse>>> GetLastTen()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.GetLastTen();
        return Ok(result.Value);
    }

    [HttpGet("{in-range}")]
    public async Task<ActionResult<IEnumerable<ConsultationReferralResponse>>> GetInRange([FromQuery] DateTime start,
    [FromQuery] DateTime end)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _consultationReferralService.GetConsultationInRange(start, end);
        return Ok(result.Value);
    } 

    [HttpPost]
    public async Task<ActionResult<ConsultationReferralResponse>> Create([FromBody] CreateConsultationReferralDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.CreateAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.ReferralId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ConsultationReferralResponse>> Update(Guid id, [FromBody] UpdateConsultationReferralDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.UpdateAsync(id, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var result = await _consultationReferralService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }

    // ============================
    // GET: api/consultationreferral/export
    // Exportar todas las consultas de referencia a PDF
    // ============================
    [HttpGet("export")]
    public async Task<ActionResult> ExportConsultationReferrals()
    {
        // Obtener todas las consultas de referencia
        var consultationsResult = await _consultationReferralService.GetAllAsync();
        if (!consultationsResult.IsSuccess)
        {
            return BadRequest(consultationsResult.ErrorMessage);
        }

        // Serializar a JSON
        string jsonData = JsonSerializer.Serialize(consultationsResult.Value);

        // Generar archivo temporal
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"consultation_referrals_{Guid.NewGuid()}.pdf");

        // Exportar usando el servicio
        var exportResult = await _exportService.ExportDataAsync(jsonData, "pdf", tempFilePath);
        
        if (!exportResult.IsSuccess)
        {
            return BadRequest(exportResult.ErrorMessage);
        }

        return Ok(exportResult.Value);
    }
}