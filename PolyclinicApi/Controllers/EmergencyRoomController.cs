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
public class EmergencyRoomController : ControllerBase
{
    private readonly IEmergencyRoomService _service;

    public EmergencyRoomController(IEmergencyRoomService service)
    {
        _service = service;
    }

    // ************************************************************
    // POST - CREATE
    // ************************************************************
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmergencyRoomDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetByIdWithDoctor),
            new { id = result.Value!.EmergencyRoomId },
            result.Value);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmergencyRoomDto dto)
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
    // GET BY ID WITH DOCTOR
    // ************************************************************
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdWithDoctor(Guid id)
    {
        var result = await _service.GetByIdWithDoctorAsync(id);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    // ************************************************************
    // GET ALL WITH DOCTOR
    // ************************************************************
    [HttpGet]
    public async Task<IActionResult> GetAllWithDoctor()
    {
        var result = await _service.GetAllWithDoctorAsync();
        return Ok(result.Value);
    }

    // ************************************************************
    // FILTER BY DATE
    // ************************************************************
    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateOnly date)
    {
        var result = await _service.GetByDateAsync(date);

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
}
