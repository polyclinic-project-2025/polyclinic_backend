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
    [ProducesResponseType(typeof(ApiResult<EmergencyRoomCareDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<EmergencyRoomCareDto>>> Create([FromBody] CreateEmergencyRoomCareDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<EmergencyRoomCareDto>.BadRequest(result.ErrorMessage!));
        var apiResult = ApiResult<EmergencyRoomCareDto>.Ok(result.Value!, "Atencion creada exitosamente");
        return CreatedAtAction(nameof(GetByIdWithDetails),new { id = result.Value!.EmergencyRoomCareId },apiResult);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateEmergencyRoomCareDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess){
            if (result.ErrorMessage!.Contains("no encontrada"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }

        return Ok(ApiResult<bool>.Ok(true, "Atencion actualizada exitosamente"));
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

        return Ok(ApiResult<bool>.Ok(true, "Atencion eliminada exitosamente"));
    }

    // ************************************************************
    // GET BY ID WITH DETAILS
    // ************************************************************
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<EmergencyRoomCareDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<EmergencyRoomCareDto>>> GetByIdWithDetails(Guid id)
    {
        var result = await _service.GetByIdWithDetailsAsync(id);

        if (!result.IsSuccess)
            return NotFound(ApiResult<EmergencyRoomCareDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<EmergencyRoomCareDto>.Ok(result.Value!, "Atencion obtenida exitosamente"));
    }

    // ************************************************************
    // GET ALL WITH DETAILS
    // ************************************************************
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetAllWithDetails()
    {
        var result = await _service.GetAllWithDetailsAsync();
        if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Error(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones obtenidas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DATE
    // ************************************************************
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetByDate([FromQuery] DateTime date)
    {
        var result = await _service.GetByDateAsync(date);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomCareDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DOCTOR NAME
    // ************************************************************
    [HttpGet("by-doctor-name")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetByDoctorName([FromQuery] string doctorName)
    {
        var result = await _service.GetByDoctorNameAsync(doctorName);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomCareDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY DOCTOR IDENTIFICATION
    // ************************************************************
    [HttpGet("by-doctor-identification")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetByDoctorIdentification([FromQuery] string doctorIdentification)
    {
        var result = await _service.GetByDoctorIdentificationAsync(doctorIdentification);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomCareDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY PATIENT NAME
    // ************************************************************
    [HttpGet("by-patient-name")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetByPatientName([FromQuery] string patientName)
    {
        var result = await _service.GetByPatientNameAsync(patientName);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomCareDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY PATIENT IDENTIFICATION
    // ************************************************************
    [HttpGet("by-patient-identification")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<EmergencyRoomCareDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<EmergencyRoomCareDto>>>> GetByPatientIdentification([FromQuery] string patientIdentification)
    {
        var result = await _service.GetByPatientIdentificationAsync(patientIdentification);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<EmergencyRoomCareDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<EmergencyRoomCareDto>>.Ok(result.Value!, "Atenciones encontradas exitosamente"));
    }
}