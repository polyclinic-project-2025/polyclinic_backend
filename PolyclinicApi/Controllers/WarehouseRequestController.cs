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
public class WarehouseRequestController : ControllerBase
{
    private readonly IWarehouseRequestService _warehouseRequestService;

    public WarehouseRequestController(IWarehouseRequestService warehouseRequestService)
    {
        _warehouseRequestService = warehouseRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetAll()
    {
        var result = await _warehouseRequestService.GetAllWarehouseRequestAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseRequestResponse>> GetById(Guid id)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetByStatus(string status)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByStatusAsync(status);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("department")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetByDepartment(Guid id)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByDepartmentIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("status-and-department")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAndDepartmentIdAsync(string status, Guid departmentId)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByStatusAndDepartmentIdAsync(status, departmentId);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseRequestResponse>> Create([FromBody] CreateWarehouseRequestRequest request)
    {
        var result = await _warehouseRequestService.CreateWarehouseRequestAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.WarehouseRequestId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequestRequest request)
    {
        var result = await _warehouseRequestService.UpdateWarehouseRequestAsync(id, request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _warehouseRequestService.DeleteWarehouseRequestAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }
}