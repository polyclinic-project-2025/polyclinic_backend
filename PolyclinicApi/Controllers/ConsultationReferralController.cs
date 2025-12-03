using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationReferralController : ControllerBase
{
    private readonly IConsultationReferralService _consultationReferralService;

    public ConsultationReferralController(IConsultationReferralService consultationReferralService)
    {
        _consultationReferralService = consultationReferralService;
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
}