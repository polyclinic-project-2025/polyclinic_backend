using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationEmergencyController : ControllerBase
{
    private readonly IMedicationEmergencyService _service;

    public MedicationEmergencyController(IMedicationEmergencyService service)
    {
        _service = service;
    }

    // ************************************************************
    // POST - CREATE
    // ************************************************************
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMedicationEmergencyDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetByIdWithMedication),
            new { id = result.Value!.MedicationEmergencyId },
            result.Value);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationEmergencyDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // DELETE
    // ************************************************************
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // GET BY ID WITH MEDICATION
    // ************************************************************
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdWithMedication(Guid id)
    {
        var result = await _service.GetByIdWithMedicationAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // GET ALL WITH MEDICATION
    // ************************************************************
    [HttpGet]
    public async Task<IActionResult> GetAllWithMedication()
    {
        var result = await _service.GetAllWithMedicationAsync();
        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY EMERGENCY ROOM CARE ID
    // ************************************************************
    [HttpGet("by-emergency-room-care/{emergencyRoomCareId:guid}")]
    public async Task<IActionResult> GetByEmergencyRoomCareId(Guid emergencyRoomCareId)
    {
        var result = await _service.GetByEmergencyRoomCareIdAsync(emergencyRoomCareId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY MEDICATION ID
    // ************************************************************
    [HttpGet("by-medication/{medicationId:guid}")]
    public async Task<IActionResult> GetByMedicationId(Guid medicationId)
    {
        var result = await _service.GetByMedicationIdAsync(medicationId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY MEDICATION NAME
    // ************************************************************
    [HttpGet("by-medication-name")]
    public async Task<IActionResult> GetByMedicationName([FromQuery] string medicationName)
    {
        var result = await _service.GetByMedicationNameAsync(medicationName);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}