using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationReferralController : ControllerBase
{
    private readonly IMedicationReferralService _service;

    public MedicationReferralController(IMedicationReferralService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicationReferralDto request)
    {
        var result = await _service.CreateAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetById),
            new { id = result.Value!.MedicationReferralId },
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationReferralDto request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }
}
