using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
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
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<DerivationDto>>.Error(result.ErrorMessage!));

            return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones obtenidas exitosamente"));
        }
    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<DerivationDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<DerivationDto>>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(ApiResult<DerivationDto>.NotFound(result.ErrorMessage!));

            return Ok(ApiResult<DerivationDto>.Ok(result.Value!, "Derivación obtenida exitosamente"));
        }

    // --------------------------------------------------------------------
    // SEARCH: Department From
    // --------------------------------------------------------------------
    [HttpGet("search/from")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> SearchByDepartmentFrom([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentFromNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<DerivationDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Department To
    // --------------------------------------------------------------------
    [HttpGet("search/to")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> SearchByDepartmentTo([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentToNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<DerivationDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Patient Name
    // --------------------------------------------------------------------
    [HttpGet("search/patient")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> SearchByPatient([FromQuery] string name)
    {
        var result = await _service.SearchByPatientNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<DerivationDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Date
    // --------------------------------------------------------------------
    [HttpGet("search/date")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> SearchByDate([FromQuery] DateTime date)
    {
        var result = await _service.SearchByDateAsync(date);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<DerivationDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones encontradas"));
    }
    [HttpGet("search/identification")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<DerivationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<DerivationDto>>>> SearchByPatientIdentification([FromQuery] string identification)
    {
        var result = await _service.SearchByPatientIdentificationAsync(identification);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<DerivationDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<DerivationDto>>.Ok(result.Value!, "Derivaciones encontradas"));
    }
    // --------------------------------------------------------------------
    // CREATE
    // --------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<DerivationDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<DerivationDto>>> Create([FromBody] CreateDerivationDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if(!result.IsSuccess)
            {
                return BadRequest(ApiResult<DerivationDto>.BadRequest(result.ErrorMessage!));
            }
        var apiResult = ApiResult<DerivationDto>.Ok(result.Value!, "Derivación creada exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.DerivationId }, apiResult);
    }

    // --------------------------------------------------------------------
    // DELETE
    // --------------------------------------------------------------------
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            if (result.ErrorMessage!.Contains("no encontrado"))
                return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
            
            return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
        }
        return Ok(ApiResult<bool>.Ok(true, "Derivación eliminada exitosamente"));
    }
    
}
}

