using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;

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
            return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamento de referencia creado exitosamente"));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<object>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamento de referencia obtenido"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamentos de referencia obtenidos"));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationReferralDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<bool>.Error(result.ErrorMessage!));

        return Ok(ApiResult<bool>.Ok(true, "Medicamento de referencia actualizado"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<bool>.Ok(true, "Medicamento de referencia eliminado"));
    }
}
