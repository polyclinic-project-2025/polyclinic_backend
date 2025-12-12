using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicationRequestController : ControllerBase
{
    private readonly IMedicationRequestService _medicationRequestService;

    public MedicationRequestController(IMedicationRequestService medicationRequestService)
    {
        _medicationRequestService = medicationRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicationRequestResponse>>> GetAll()
    {
        var result = await _medicationRequestService.GetAllMedicationRequestAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MedicationRequestResponse>> GetById(Guid id)
    {
        var result = await _medicationRequestService.GetMedicationRequestByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("warehouse-request")]
    public async Task<ActionResult<IEnumerable<MedicationRequestResponse>>> GetByWarehouseRequestId(Guid warehouseRequestId)
    {
        var result = await _medicationRequestService.GetMedicationRequestByWarehouseRequestIdAsync(warehouseRequestId);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<MedicationRequestResponse>> Create([FromBody] CreateMedicationRequestRequest request)
    {
        var result = await _medicationRequestService.CreateMedicationRequestAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.MedicationRequestId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateMedicationRequestRequest request)
    {
        var result = await _medicationRequestService.UpdateMedicationRequestAsync(id, request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _medicationRequestService.DeleteMedicationRequestAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }
}