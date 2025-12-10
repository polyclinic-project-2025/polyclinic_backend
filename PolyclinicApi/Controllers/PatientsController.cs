using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients.Value);
        }

        // ============================
        // GET: api/patients/{id}
        // Obtener paciente por Id
        // ============================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if(!patient.IsSuccess)
            {
                return NotFound(patient.ErrorMessage);
            }
            return Ok(patient.Value);
        }

        // ============================
        // GET: api/patients/search
        // Buscar pacientes por nombre, identificación o edad
        // ============================
        [HttpGet("search/name")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchByName([FromQuery] string name)
        {
            var byName = await _patientService.GetByNameAsync(name);
            if(!byName.IsSuccess)
            {
                return NotFound(byName.ErrorMessage);
            }
            return Ok(byName.Value);
        }

        [HttpGet("search/identification")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchByIdentification([FromQuery] string identification)
        {
            var byIdentification = await _patientService.GetByIdentificationAsync(identification);
            if(!byIdentification.IsSuccess)
            {
                return NotFound(byIdentification.ErrorMessage);
            }
            return Ok(byIdentification.Value);
        }
        [HttpGet("search/age")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> SearchByAge([FromQuery] int age)
        {
            var byAge = await _patientService.GetByAgeAsync(age);
            if(!byAge.IsSuccess)
            {
                return NotFound(byAge.ErrorMessage);
            }
            return Ok(byAge.Value);
        }
        // ============================
        // POST: api/patients
        // Crear paciente
        // ============================
        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
        {
            var result = await _patientService.CreateAsync(dto);
            if(!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Value.PatientId }, result.Value);
        }

        // ============================
        // PUT: api/patients/{id}
        // Actualización parcial
        // ============================
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            var result = await _patientService.UpdateAsync(id, dto);
            if(!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return NoContent();
        }

        // ================ot============
        // DELETE: api/patients/{id}
        // Eliminar paciente
        // ============================
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _patientService.DeleteAsync(id);
        if(!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }
        return NoContent();
    }

        // ============================
        // GET: api/patients/export
        // Exportar todos los pacientes a PDF
        // ============================
        [HttpGet("export")]
        public async Task<ActionResult> ExportPatients()
        {
            // Obtener todos los pacientes
            var patientsResult = await _patientService.GetAllAsync();
            if (!patientsResult.IsSuccess)
            {
                return BadRequest(patientsResult.ErrorMessage);
            }

            // Serializar a JSON
            string jsonData = JsonSerializer.Serialize(patientsResult.Value);

            // Generar archivo temporal
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"patients_{Guid.NewGuid()}.pdf");

            // Exportar usando el servicio
            var exportResult = await _exportService.ExportDataAsync(jsonData, "pdf", tempFilePath);
            
            if (!exportResult.IsSuccess)
            {
                return BadRequest(exportResult.ErrorMessage);
            }

            return Ok(exportResult.Value);
        }
    }
}
