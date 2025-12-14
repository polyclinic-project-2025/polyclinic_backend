using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;

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
    [ProducesResponseType(typeof(ApiResult<EmergencyRoomDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<EmergencyRoomDto>>> Create([FromBody] CreateEmergencyRoomDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<EmergencyRoomDto>.BadRequest(result.ErrorMessage!));
        var apiResult = ApiResult<EmergencyRoomDto>.Ok(result.Value!, "Guardia creada exitosamente");
        return CreatedAtAction(nameof(GetByIdWithDoctor),new { id = result.Value!.EmergencyRoomId }, apiResult);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateEmergencyRoomDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess){
            if (result.ErrorMessage!.Contains("no encontrada"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        

        return Ok(ApiResult<bool>.Ok(true, "Guardia actualizada exitosamente"));
    }

    // ************************************************************
    // DELETE
    // ************************************************************
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess){
            if (result.ErrorMessage!.Contains("no encontrada"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }

        return Ok(ApiResult<bool>.Ok(true, "Guardia eliminada exitosamente"));
    }

    // ************************************************************
    // GET BY ID WITH DOCTOR
    // ************************************************************
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<EmergencyRoomDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<EmergencyRoomDto>>> GetByIdWithDoctor(Guid id)
    {
        var result = await _service.GetByIdWithDoctorAsync(id);

        if (!result.IsSuccess)
            return NotFound(ApiResult<EmergencyRoomDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<EmergencyRoomDto>.Ok(result.Value!, "Guardia obtenida exitosamente"));
    }

    // ************************************************************
    // GET ALL WITH DOCTOR
    // ************************************************************
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomDto>>>> GetAllWithDoctor()
    {
        var result = await _service.GetAllWithDoctorAsync();
        if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<EmergencyRoomDto>>.Error(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<EmergencyRoomDto>>.Ok(result.Value!, "Guardias obtenidas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DATE
    // ************************************************************
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomDto>>>> GetByDate([FromQuery] DateOnly date)
    {
        var result = await _service.GetByDateAsync(date);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomDto>>.Ok(result.Value!, "Guardias encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DOCTOR IDENTIFICATION
    // ************************************************************
    [HttpGet("by-doctor-identification")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomDto>>>> GetByDoctorIdentification([FromQuery] string doctorIdentification)
    {
        var result = await _service.GetByDoctorIdentificationAsync(doctorIdentification);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomDto>>.Ok(result.Value!, "Guardias encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DOCTOR NAME
    // ************************************************************
    [HttpGet("by-doctor-name")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomDto>>>> GetByDoctorName([FromQuery] string doctorName)
    {
        var result = await _service.GetByDoctorNameAsync(doctorName);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomDto>>.Ok(result.Value!, "Guardias encontradas exitosamente"));
    }
}
