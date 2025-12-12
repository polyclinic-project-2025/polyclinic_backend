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
public class EmergencyRoomCareController : ControllerBase
{
    private readonly IEmergencyRoomCareService _service;

    public EmergencyRoomCareController(IEmergencyRoomCareService service)
    {
        _service = service;
    }

    // ************************************************************
    // POST - CREATE
    // ************************************************************
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmergencyRoomCareDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetByIdWithDetails),
            new { id = result.Value!.EmergencyRoomCareId },
            result.Value);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmergencyRoomCareDto dto)
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
    // GET BY ID WITH DETAILS
    // ************************************************************
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdWithDetails(Guid id)
    {
        var result = await _service.GetByIdWithDetailsAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // GET ALL WITH DETAILS
    // ************************************************************
    [HttpGet]
    public async Task<IActionResult> GetAllWithDetails()
    {
        var result = await _service.GetAllWithDetailsAsync();
        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY DATE
    // ************************************************************
    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var result = await _service.GetByDateAsync(date);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY DOCTOR NAME
    // ************************************************************
    [HttpGet("by-doctor-name")]
    public async Task<IActionResult> GetByDoctorName([FromQuery] string doctorName)
    {
        var result = await _service.GetByDoctorNameAsync(doctorName);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY DOCTOR IDENTIFICATION
    // ************************************************************
    [HttpGet("by-doctor-identification")]
    public async Task<IActionResult> GetByDoctorIdentification([FromQuery] string doctorIdentification)
    {
        var result = await _service.GetByDoctorIdentificationAsync(doctorIdentification);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY PATIENT NAME
    // ************************************************************
    [HttpGet("by-patient-name")]
    public async Task<IActionResult> GetByPatientName([FromQuery] string patientName)
    {
        var result = await _service.GetByPatientNameAsync(patientName);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY PATIENT IDENTIFICATION
    // ************************************************************
    [HttpGet("by-patient-identification")]
    public async Task<IActionResult> GetByPatientIdentification([FromQuery] string patientIdentification)
    {
        var result = await _service.GetByPatientIdentificationAsync(patientIdentification);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }
}