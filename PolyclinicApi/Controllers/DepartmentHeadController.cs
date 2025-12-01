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
public class DepartmentHeadController : ControllerBase
{
    private readonly IDepartmentHeadService _departmentHeadService;

    public DepartmentHeadController(IDepartmentHeadService departmentHeadService)
    {
        _departmentHeadService = departmentHeadService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentHeadResponse>>> GetAll()
    {
        var result = await _departmentHeadService.GetAllDepartmentHeadAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentHeadResponse>> GetById(Guid id)
    {
        var result = await _departmentHeadService.GetDepartmentHeadByIdAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpGet("by-department-id/{departmentId:guid}")]
    public async Task<ActionResult<DepartmentHeadResponse>> GetByDepartmentId(Guid departmentId)
    {
        var result = await _departmentHeadService.GetDepartmentHeadByDepartmentIdAsync(departmentId);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentHeadResponse>> AssignDepartmentHead([FromBody] AssignDepartmentHeadRequest request)
    {
        var result = await _departmentHeadService.AssignDepartmentHeadAsync(request);
        if(!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.DepartmentHeadId }, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveDepartmentHead(Guid id)
    {
        var result = await _departmentHeadService.RemoveDepartmentHeadAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }
}