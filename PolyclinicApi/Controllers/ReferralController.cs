using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
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
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<ReferralDto>>.Error(result.ErrorMessage!));

            return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias obtenidas exitosamente"));
        }
    // --------------------------------------------------------------------
    // GET BY ID
    // --------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<ReferralDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<ReferralDto>>> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(ApiResult<ReferralDto>.NotFound(result.ErrorMessage!));
            return Ok(ApiResult<ReferralDto>.Ok(result.Value!, "Referencia obtenida exitosamente"));
        }

    // --------------------------------------------------------------------
    // SEARCH: Puesto Externo
    // --------------------------------------------------------------------
    [HttpGet("search/from")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> SearchByPuestoExterno([FromQuery] string name)
    {
        var result = await _service.SearchByPuestoExternoAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<ReferralDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Department To
    // --------------------------------------------------------------------
    [HttpGet("search/to")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> SearchByDepartmentTo([FromQuery] string name)
    {
        var result = await _service.SearchByDepartmentToNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<ReferralDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Patient Name
    // --------------------------------------------------------------------
    [HttpGet("search/patient")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> SearchByPatient([FromQuery] string name)
    {
        var result = await _service.SearchByPatientNameAsync(name);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<ReferralDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias encontradas"));
    }

    // --------------------------------------------------------------------
    // SEARCH: Date
    // --------------------------------------------------------------------
    [HttpGet("search/date")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> SearchByDate([FromQuery] DateTime date)
    {
        var result = await _service.SearchByDateAsync(date);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<ReferralDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias encontradas"));
    }
    [HttpGet("search/identification")]
    [ProducesResponseType(typeof(ApiResult<IEnumerable<ReferralDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    public async Task<ActionResult<ApiResult<IEnumerable<ReferralDto>>>> SearchByPatientIdentification([FromQuery] string identification)
    {
        var result = await _service.SearchByPatientIdentificationAsync(identification);
        if (!result.IsSuccess)
                return NotFound(ApiResult<IEnumerable<ReferralDto>>.NotFound(result.ErrorMessage!));
        return Ok(ApiResult<IEnumerable<ReferralDto>>.Ok(result.Value!, "Referencias encontradas"));
    }
    // --------------------------------------------------------------------
    // CREATE
    // --------------------------------------------------------------------
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<ReferralDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    public async Task<ActionResult<ApiResult<ReferralDto>>> Create([FromBody] CreateReferralDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if(!result.IsSuccess)
            {
                return BadRequest(ApiResult<ReferralDto>.BadRequest(result.ErrorMessage!));
            }
        var apiResult = ApiResult<ReferralDto>.Ok(result.Value!, "Referencia creada exitosamente");
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.ReferralId }, apiResult);
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
        return Ok(ApiResult<bool>.Ok(true, "Referencia eliminada exitosamente"));
    }
}
}