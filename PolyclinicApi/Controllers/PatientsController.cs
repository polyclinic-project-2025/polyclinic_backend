using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // ============================
        // GET: api/patients
        // Obtener todos los pacientes
        // ============================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        // ============================
        // GET: api/patients/{id}
        // Obtener paciente por Id
        // ============================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
                return NotFound("Paciente no encontrado.");

            return Ok(patient);
        }

        // ============================
        // GET: api/patients/{id}/with-relations
        // Obtener paciente con relaciones
        // ============================
        /*[HttpGet("{id:guid}/with-relations")]
        public async Task<ActionResult<Patient>> GetWithRelations(Guid id)
        {
            var patient = await _patientService.GetWithRelationsAsync(id);
            if (patient == null)
                return NotFound("Paciente no encontrado.");

            return Ok(patient);
        }*/

        // ============================
        // GET: api/patients/search
        // Buscar pacientes por nombre, identificación o edad
        // ============================
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> Search(
            [FromQuery] string? name,
            [FromQuery] string? identification,
            [FromQuery] int? age)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var byName = await _patientService.GetByNameAsync(name);
                return Ok(byName);
            }

            if (!string.IsNullOrWhiteSpace(identification))
            {
                var byId = await _patientService.GetByIdentificationAsync(identification);
                return Ok(byId);
            }

            if (age.HasValue)
            {
                var byAge = await _patientService.GetByAgeAsync(age.Value);
                return Ok(byAge);
            }

            return BadRequest("Debe proporcionar al menos un criterio de búsqueda: name, identification o age.");
        }

        // ============================
        // POST: api/patients
        // Crear paciente
        // ============================
        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
        {
            try
            {
                var created = await _patientService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.PatientId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================
        // PUT: api/patients/{id}
        // Actualización parcial
        // ============================
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            try
            {
                await _patientService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ============================
        // DELETE: api/patients/{id}
        // Eliminar paciente
        // ============================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _patientService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
