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
    [ProducesResponseType(typeof(ApiResult<MedicationEmergencyDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<MedicationEmergencyDto>>> Create([FromBody] CreateMedicationEmergencyDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(ApiResult<MedicationEmergencyDto>.BadRequest(result.ErrorMessage!));
        var apiResult = ApiResult<MedicationEmergencyDto>.Ok(result.Value!, "Medicacion creada exitosamente");
        return CreatedAtAction(nameof(GetByIdWithMedication),new { id = result.Value!.MedicationEmergencyId },apiResult);
    }

    // ************************************************************
    // PUT - UPDATE
    // ************************************************************
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateMedicationEmergencyDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess){
            if (result.ErrorMessage!.Contains("no encontrada"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
            

        return Ok(ApiResult<bool>.Ok(true, "Medicacion actualizada exitosamente"));
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

        return Ok(ApiResult<bool>.Ok(true, "Medicacion eliminada exitosamente"));
    }

    // ************************************************************
    // GET BY ID WITH MEDICATION
    // ************************************************************
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<MedicationEmergencyDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<MedicationEmergencyDto>>> GetByIdWithMedication(Guid id)
    {
        var result = await _service.GetByIdWithMedicationAsync(id);

        if (!result.IsSuccess)
            return NotFound(ApiResult<MedicationEmergencyDto>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<MedicationEmergencyDto>.Ok(result.Value!, "Medicacion obtenida exitosamente"));
    }

    // ************************************************************
    // GET ALL WITH MEDICATION
    // ************************************************************
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationEmergencyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationEmergencyDto>>>> GetAllWithMedication()
    {
        var result = await _service.GetAllWithMedicationAsync();
        if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<MedicationEmergencyDto>>.Error(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<MedicationEmergencyDto>>.Ok(result.Value!, "Medicaciones obtenidas exitosamente"));
    }

    // ************************************************************
    // FILTER BY EMERGENCY ROOM CARE ID
    // ************************************************************
    [HttpGet("by-emergency-room-care/{emergencyRoomCareId:guid}")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationEmergencyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationEmergencyDto>>>> GetByEmergencyRoomCareId(Guid emergencyRoomCareId)
    {
        var result = await _service.GetByEmergencyRoomCareIdAsync(emergencyRoomCareId);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<MedicationEmergencyDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationEmergencyDto>>.Ok(result.Value!, "Medicaciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY MEDICATION ID
    // ************************************************************
    [HttpGet("by-medication/{medicationId:guid}")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationEmergencyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationEmergencyDto>>>> GetByMedicationId(Guid medicationId)
    {
        var result = await _service.GetByMedicationIdAsync(medicationId);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<MedicationEmergencyDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationEmergencyDto>>.Ok(result.Value!, "Medicaciones encontradas exitosamente"));
    }

    // ************************************************************
    // FILTER BY MEDICATION NAME
    // ************************************************************
    [HttpGet("by-medication-name")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<MedicationEmergencyDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<MedicationEmergencyDto>>>> GetByMedicationName([FromQuery] string medicationName)
    {
        var result = await _service.GetByMedicationNameAsync(medicationName);

        if (!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<MedicationEmergencyDto>>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<MedicationEmergencyDto>>.Ok(result.Value!, "Medicaciones encontradas exitosamente"));
    }
}