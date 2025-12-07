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
public class WarehouseManagerController : ControllerBase
{
    private readonly IWarehouseManagerService _warehouseManagerService;

    public WarehouseManagerController(IWarehouseManagerService warehouseManagerService)
    {
        _warehouseManagerService = warehouseManagerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseManagerResponse>>> GetAll()
    {
        var result = await _warehouseManagerService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseManagerResponse>> GetById(Guid id)
    {
        var result = await _warehouseManagerService.GetByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("manager")]
    public async Task<ActionResult<WarehouseManagerResponse>> GetWarehouseManager()
    {
        var result = await _warehouseManagerService.GetWarehouseManagerAsync();
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseManagerResponse>> Create([FromBody] CreateWarehouseManagerRequest request)
    {
        var result = await _warehouseManagerService.CreateAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.EmployeeId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateWarehouseManagerRequest request)
    {
        var result = await _warehouseManagerService.UpdateAsync(id, request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _warehouseManagerService.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }
}