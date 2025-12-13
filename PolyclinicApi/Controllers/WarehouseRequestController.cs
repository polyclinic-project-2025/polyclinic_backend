using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;

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
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<WarehouseRequestResponse>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<WarehouseRequestResponse>>.Ok(result.Value!, "Solicitudes de almacén obtenidas"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WarehouseRequestResponse>> GetById(Guid id)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByIdAsync(id);
        if(!result.IsSuccess)
            return NotFound(ApiResult<WarehouseRequestResponse>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<WarehouseRequestResponse>.Ok(result.Value!, "Solicitud obtenida"));
    }

    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetByStatus(string status)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByStatusAsync(status);
        if(!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<WarehouseRequestResponse>>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<WarehouseRequestResponse>>.Ok(result.Value!, "Solicitudes obtenidas"));
    }

    [HttpGet("department")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetByDepartment(Guid id)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByDepartmentIdAsync(id);
        if(!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<WarehouseRequestResponse>>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<WarehouseRequestResponse>>.Ok(result.Value!, "Solicitudes obtenidas"));
    }

    [HttpGet("status-and-department")]
    public async Task<ActionResult<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAndDepartmentIdAsync(string status, Guid departmentId)
    {
        var result = await _warehouseRequestService.GetWarehouseRequestByStatusAndDepartmentIdAsync(status, departmentId);
        if(!result.IsSuccess)
            return NotFound(ApiResult<IEnumerable<WarehouseRequestResponse>>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<WarehouseRequestResponse>>.Ok(result.Value!, "Solicitudes obtenidas"));
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseRequestResponse>> Create([FromBody] CreateWarehouseRequestRequest request)
    {
        var result = await _warehouseRequestService.CreateWarehouseRequestAsync(request);
        if(!result.IsSuccess)
            return BadRequest(ApiResult<WarehouseRequestResponse>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<WarehouseRequestResponse>.Ok(result.Value!, "Solicitud creada exitosamente"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequestRequest request)
    {
        var result = await _warehouseRequestService.UpdateWarehouseRequestAsync(id, request);
        if(!result.IsSuccess)
            return BadRequest(ApiResult<bool>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<bool>.Ok(true, "Solicitud actualizada"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _warehouseRequestService.DeleteWarehouseRequestAsync(id);
        if(!result.IsSuccess)
            return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage!));
        
        return Ok(ApiResult<bool>.Ok(true, "Solicitud eliminada"));
    }
}