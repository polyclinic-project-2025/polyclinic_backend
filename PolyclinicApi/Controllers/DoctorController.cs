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
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DoctorResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<DoctorResponse>>>> GetAll()
    {
        var result = await _doctorService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<DoctorResponse>>.Error(result.ErrorMessage!));

        return Ok(ApiResult<IEnumerable<DoctorResponse>>.Ok(result.Value!, "Doctores obtenidos exitosamente"));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<DoctorResponse>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<DoctorResponse>>> GetById(Guid id)
    {
        var result = await _doctorService.GetByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(ApiResult<DoctorResponse>.NotFound(result.ErrorMessage!));
        }
        return Ok(ApiResult<DoctorResponse>.Ok(result.Value!, "Doctor obtenido exitosamente"));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<DoctorResponse>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<DoctorResponse>>> Create([FromBody] CreateDoctorRequest request)
    {
        var result = await _doctorService.CreateAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(ApiResult<DoctorResponse>.BadRequest(result.ErrorMessage!));
        }
        var apiResult = ApiResult<DoctorResponse>.Ok(result.Value!, "Doctor creado exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.EmployeeId }, apiResult);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdateDoctorRequest request)
    {
        var result = await _doctorService.UpdateAsync(id, request);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Doctor actualizado exitosamente"));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
    {
        var result = await _doctorService.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Doctor eliminado exitosamente"));
    }
}