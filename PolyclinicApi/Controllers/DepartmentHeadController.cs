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
public class DepartmentHeadController : ControllerBase
{
    private readonly IDepartmentHeadService _departmentHeadService;

    public DepartmentHeadController(IDepartmentHeadService departmentHeadService)
    {
        _departmentHeadService = departmentHeadService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DepartmentHeadResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<DepartmentHeadResponse>>>> GetAll()
    {
        var result = await _departmentHeadService.GetAllDepartmentHeadAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<DepartmentHeadResponse>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<DepartmentHeadResponse>>.Ok(result.Value!, "Jefes de departamento obtenidos exitosamente"));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<DepartmentHeadResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<DepartmentHeadResponse>>> GetById(Guid id)
    {
        var result = await _departmentHeadService.GetDepartmentHeadByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<DepartmentHeadResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<DepartmentHeadResponse>.Ok(result.Value!, "Jefe de departamento obtenido exitosamente"));
    }

    [HttpGet("by-department-id/{departmentId:guid}")]
    [ProducesResponseType(typeof(ApiResult<DepartmentHeadResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<DepartmentHeadResponse>>> GetByDepartmentId(Guid departmentId)
    {
        var result = await _departmentHeadService.GetDepartmentHeadByDepartmentIdAsync(departmentId);
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<DepartmentHeadResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<DepartmentHeadResponse>.Ok(result.Value!, "Jefe de departamento encontrado"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<DepartmentHeadResponse>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<DepartmentHeadResponse>>> AssignDepartmentHead([FromBody] AssignDepartmentHeadRequest request)
    {
        var result = await _departmentHeadService.AssignDepartmentHeadAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(ApiResult<DepartmentHeadResponse>.BadRequest(result.ErrorMessage!));
        }
        var apiResult = ApiResult<DepartmentHeadResponse>.Ok(result.Value!, "Jefe de departamento asignado exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.DepartmentHeadId }, apiResult);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> RemoveDepartmentHead(Guid id)
    {
        var result = await _departmentHeadService.RemoveDepartmentHeadAsync(id);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Jefe de departamento removido exitosamente"));
    }
}