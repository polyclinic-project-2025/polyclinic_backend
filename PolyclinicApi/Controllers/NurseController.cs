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
public class NurseController : ControllerBase
{
    private readonly INurseService _nurseService;

    public NurseController(INurseService nurseService)
    {
        _nurseService = nurseService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<NurseResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<NurseResponse>>>> GetAll()
    {
        var result = await _nurseService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<NurseResponse>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<NurseResponse>>.Ok(result.Value!, "Enfermeras obtenidas exitosamente"));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<NurseResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<NurseResponse>>> GetById(Guid id)
    {
        var result = await _nurseService.GetByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<NurseResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<NurseResponse>.Ok(result.Value!, "Enfermera obtenida exitosamente"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<NurseResponse>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<NurseResponse>>> Create([FromBody] CreateNurseRequest request)
    {
        var result = await _nurseService.CreateAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(ApiResult<NurseResponse>.BadRequest(result.ErrorMessage!));
        }
        var apiResult = ApiResult<NurseResponse>.Ok(result.Value!, "Enfermera creada exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.EmployeeId }, apiResult);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateNurseRequest request)
    {
        var result = await _nurseService.UpdateAsync(id, request);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Enfermera actualizada exitosamente"));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
    {
        var result = await _nurseService.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Enfermera eliminada exitosamente"));
    }
}