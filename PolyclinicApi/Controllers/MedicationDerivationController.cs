using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicApplication.Common.Results;


namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationDerivationController : ControllerBase
{
    private readonly IMedicationDerivationService _service;

    public MedicationDerivationController(IMedicationDerivationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicationDerivationDto request)
    {
        var result = await _service.CreateAsync(request);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamento de derivación creado exitosamente"));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<object>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamento de derivación obtenido"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamentos de derivación obtenidos"));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDerivationDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<object>.Error(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(result.Value!, "Medicamento de derivación actualizado"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<bool>.Ok(true, "Medicamento de derivación eliminado"));
    }
}