using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Export;
using PolyclinicApplication.DTOs.Response.Patients;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IExportService _exportService;

        public PatientsController(IPatientService patientService, IExportService exportService)
        {
            _patientService = patientService;
            _exportService = exportService;
        }

        // ============================
        // GET: api/patients
        // Obtener todos los pacientes
        // ============================
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<PatientDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<IEnumerable<PatientDto>>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            if (!patients.IsSuccess)
                return BadRequest(ApiResult<IEnumerable<PatientDto>>.Error(patients.ErrorMessage!));

            return Ok(ApiResult<IEnumerable<PatientDto>>.Ok(patients.Value!, "Pacientes obtenidos exitosamente"));
        }

        // ============================
        // GET: api/patients/{id}
        // Obtener paciente por Id
        // ============================
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<PatientDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<PatientDto>>> GetById(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if(!patient.IsSuccess)
            {
                return NotFound(ApiResult<PatientDto>.NotFound(patient.ErrorMessage!));
            }
            return Ok(ApiResult<PatientDto>.Ok(patient.Value!, "Paciente obtenido exitosamente"));
        }

        // ============================
        // GET: api/patients/search
        // Buscar pacientes por nombre, identificación o edad
        // ============================
        [HttpGet("search/name")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<PatientDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<IEnumerable<PatientDto>>>> SearchByName([FromQuery] string name)
        {
            var byName = await _patientService.GetByNameAsync(name);
            if(!byName.IsSuccess)
            {
                return NotFound(ApiResult<IEnumerable<PatientDto>>.NotFound(byName.ErrorMessage!));
            }
            return Ok(ApiResult<IEnumerable<PatientDto>>.Ok(byName.Value!, "Pacientes encontrados"));
        }

        [HttpGet("search/identification")]
        [ProducesResponseType(typeof(ApiResult<PatientDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<PatientDto>>> SearchByIdentification([FromQuery] string identification)
        {
            var byIdentification = await _patientService.GetByIdentificationAsync(identification);
            if(!byIdentification.IsSuccess)
            {
                return NotFound(ApiResult<PatientDto>.NotFound(byIdentification.ErrorMessage!));
            }
            return Ok(ApiResult<PatientDto>.Ok(byIdentification.Value!, "Paciente encontrado"));
        }
        [HttpGet("search/age")]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<PatientDto>>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<IEnumerable<PatientDto>>>> SearchByAge([FromQuery] int age)
        {
            var byAge = await _patientService.GetByAgeAsync(age);
            if(!byAge.IsSuccess)
            {
                return NotFound(ApiResult<IEnumerable<PatientDto>>.NotFound(byAge.ErrorMessage!));
            }
            return Ok(ApiResult<IEnumerable<PatientDto>>.Ok(byAge.Value!, "Pacientes encontrados"));
        }
        // ============================
        // POST: api/patients
        // Crear paciente
        // ============================
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<PatientDto>), 201)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<PatientDto>>> Create([FromBody] CreatePatientDto dto)
        {
            var result = await _patientService.CreateAsync(dto);
            if(!result.IsSuccess)
            {
                return BadRequest(ApiResult<PatientDto>.BadRequest(result.ErrorMessage!));
            }
            var apiResult = ApiResult<PatientDto>.Ok(result.Value!, "Paciente creado exitosamente");
            return CreatedAtAction(nameof(GetById), new { id = result.Value!.PatientId }, apiResult);
        }

        // ============================
        // PUT: api/patients/{id}
        // Actualización parcial
        // ============================
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            var result = await _patientService.UpdateAsync(id, dto);
            if(!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }
            return Ok(ApiResult<bool>.Ok(true, "Paciente actualizado exitosamente"));
        }

        // ============================
        // DELETE: api/patients/{id}
        // Eliminar paciente
        // ============================
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResult<bool>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 404)]
        public async Task<ActionResult<ApiResult<bool>>> Delete(Guid id)
        {
            var result = await _patientService.DeleteAsync(id);
            if(!result.IsSuccess)
            {
                if (result.ErrorMessage!.Contains("no encontrado"))
                    return NotFound(ApiResult<bool>.NotFound(result.ErrorMessage));
                
                return BadRequest(ApiResult<bool>.BadRequest(result.ErrorMessage));
            }
            return Ok(ApiResult<bool>.Ok(true, "Paciente eliminado exitosamente"));
        }

        // ============================
        // GET: api/patients/export
        // Exportar todos los pacientes a PDF
        // ============================
        [HttpGet("export")]
        [ProducesResponseType(typeof(ApiResult<ExportResponse>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        public async Task<ActionResult<ApiResult<ExportResponse>>> ExportPatients()
        {
            // Obtener todos los pacientes
            var patientsResult = await _patientService.GetAllAsync();
            if (!patientsResult.IsSuccess)
            {
                return BadRequest(ApiResult<ExportResponse>.Error(patientsResult.ErrorMessage!));
            }

            // Serializar a JSON
            string jsonData = JsonSerializer.Serialize(patientsResult.Value);

            // Generar archivo temporal
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"patients_{Guid.NewGuid()}.pdf");

            // Exportar usando el servicio
            var exportResult = await _exportService.ExportDataAsync(jsonData, "pdf", tempFilePath);
            
            if (!exportResult.IsSuccess)
            {
                return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));
            }

            return Ok(ApiResult<ExportResponse>.Ok(exportResult.Value!, "Pacientes exportados exitosamente"));
        }
    }
}
