using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Referral;
using PolyclinicApplication.DTOs.Response.Referral;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicApi.Controllers{
    [ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _service;

    public ReferralController(IReferralService service)
    {
        _service = service;
    }

    // --------------------------------------------------------------------
    // GET ALL
    // --------------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Value);
        }
    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReferralDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
            return Ok(result.Value);
        }

    // --------------------------------------------------------------------
    // SEARCH: Puesto Externo
    // --------------------------------------------------------------------
    [HttpGet("search/from")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPuestoExterno([FromQuery] string name)
    {
        var result = await _service.SearchByPuestoExternoAsync(name);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }

    // --------------------------------------------------------------------
    // SEARCH: Department To
    // --------------------------------------------------------------------
    [HttpGet("search/to")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByDepartmentTo([FromQuery] string name)
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
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPatient([FromQuery] string name)
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
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByDate([FromQuery] DateTime date)
    {
        var result = await _service.SearchByDateAsync(date);
        if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);
        return Ok(result.Value);
    }
    [HttpGet("search/identification")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPatientIdentification([FromQuery] string identification)
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
    public async Task<ActionResult<ReferralDto>> Create([FromBody] CreateReferralDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if(!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
        return CreatedAtAction(nameof(GetById), new { id = result.Value.ReferralId }, result.Value);
    }

    // --------------------------------------------------------------------
    // DELETE
    // --------------------------------------------------------------------
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
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