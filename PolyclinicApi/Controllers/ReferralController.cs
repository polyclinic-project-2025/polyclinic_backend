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
            return Ok(result);
        }
    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReferralDto>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null)
                return NotFound("Paciente no encontrado.");

            return Ok(result);
        }

    // --------------------------------------------------------------------
    // SEARCH: Puesto Externo
    // --------------------------------------------------------------------
    [HttpGet("search/from")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPuestoExterno([FromQuery] string name)
    {
        var result = await _service.SearchByPuestoExternoAsync(name);
        return Ok(result);
    }

    // --------------------------------------------------------------------
    // SEARCH: Department To
    // --------------------------------------------------------------------
    [HttpGet("search/to")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByDepartmentTo([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentToNameAsync(name);
        return Ok(result);
    }

    // --------------------------------------------------------------------
    // SEARCH: Patient Name
    // --------------------------------------------------------------------
    [HttpGet("search/patient")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPatient([FromQuery] string name)
    {
        var result = await _service.SearchByPatientNameAsync(name);
        return Ok(result);
    }

    // --------------------------------------------------------------------
    // SEARCH: Date
    // --------------------------------------------------------------------
    [HttpGet("search/date")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByDate([FromQuery] DateTime date)
    {
        var result = await _service.SearchByDateAsync(date);
        return Ok(result);
    }
    [HttpGet("search/identification")]
    public async Task<ActionResult<IEnumerable<ReferralDto>>> SearchByPatientIdentification([FromQuery] string identification)
    {
        var result = await _service.SearchByPatientIdentificationAsync(identification);
        return Ok(result);
    }
    // --------------------------------------------------------------------
    // CREATE
    // --------------------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult<ReferralDto>> Create([FromBody] CreateReferralDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.ReferralId }, result);
    }

    // --------------------------------------------------------------------
    // DELETE
    // --------------------------------------------------------------------
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
    }
}
}