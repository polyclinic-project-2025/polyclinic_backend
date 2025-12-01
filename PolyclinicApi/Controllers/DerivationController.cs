using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicApi.Controllers{
    [ApiController]
[Route("api/[controller]")]
public class DerivationController : ControllerBase
{
    private readonly IDerivationService _service;

    public DerivationController(IDerivationService service)
    {
        _service = service;
    }

    // --------------------------------------------------------------------
    // GET ALL
    // --------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Value);
        }
    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DerivationDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(result.Value);
        }

    // --------------------------------------------------------------------
    // SEARCH: Department From
    // --------------------------------------------------------------------
    [HttpGet("search/from")]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> SearchByDepartmentFrom([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentFromNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // SEARCH: Department To
    // --------------------------------------------------------------------
    [HttpGet("search/to")]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> SearchByDepartmentTo([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentToNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // SEARCH: Patient Name
    // --------------------------------------------------------------------
    [HttpGet("search/patient")]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> SearchByPatient([FromQuery] string name)
    {
        var result = await _service.SearchByPatientNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // SEARCH: Date
    // --------------------------------------------------------------------
    [HttpGet("search/date")]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> SearchByDate([FromQuery] DateTime date)
    {
        var result = await _service.SearchByDateAsync(date);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }
    [HttpGet("search/identification")]
    public async Task<ActionResult<IEnumerable<DerivationDto>>> SearchByPatientIdentification([FromQuery] string identification)
    {
        var result = await _service.SearchByPatientIdentificationAsync(identification);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }
    // --------------------------------------------------------------------
    // CREATE
    // --------------------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult<DerivationDto>> Create([FromBody] CreateDerivationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if(!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.DerivationId }, result.Value);
    }

    // --------------------------------------------------------------------
    // DELETE
    // --------------------------------------------------------------------
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }
    
}
}

