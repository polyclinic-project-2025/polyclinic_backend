using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
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
    [ProducesResponseType(typeof(ApiResult<IEnumerable<WarehouseManagerResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<WarehouseManagerResponse>>>> GetAll()
    {
        var result = await _warehouseManagerService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<WarehouseManagerResponse>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<WarehouseManagerResponse>>.Ok(result.Value!, "Administradores de almacén obtenidos exitosamente"));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<WarehouseManagerResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<WarehouseManagerResponse>>> GetById(Guid id)
    {
        var result = await _warehouseManagerService.GetByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<WarehouseManagerResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<WarehouseManagerResponse>.Ok(result.Value!, "Administrador de almacén obtenido exitosamente"));
    }

    [HttpGet("manager")]
    [ProducesResponseType(typeof(ApiResult<WarehouseManagerResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<WarehouseManagerResponse>>> GetWarehouseManager()
    {
        var result = await _warehouseManagerService.GetWarehouseManagerAsync();
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<WarehouseManagerResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<WarehouseManagerResponse>.Ok(result.Value!, "Administrador de almacén encontrado"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<WarehouseManagerResponse>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<WarehouseManagerResponse>>> Create([FromBody] CreateWarehouseManagerRequest request)
    {
        var result = await _warehouseManagerService.CreateAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(ApiResult<WarehouseManagerResponse>.BadRequest(result.ErrorMessage!));
        }
        var apiResult = ApiResult<WarehouseManagerResponse>.Ok(result.Value!, "Administrador de almacén creado exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.EmployeeId }, apiResult);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateWarehouseManagerRequest request)
    {
        var result = await _warehouseManagerService.UpdateAsync(id, request);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Administrador de almacén actualizado exitosamente"));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
    {
        var result = await _warehouseManagerService.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Administrador de almacén eliminado exitosamente"));
    }
}