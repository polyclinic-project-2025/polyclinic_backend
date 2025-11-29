using AutoMapper;
using FluentValidation;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePatientDto> _createValidator;
        private readonly IValidator<UpdatePatientDto> _updateValidator;

        public PatientService(
            IPatientRepository repository,
            IMapper mapper,
            IValidator<CreatePatientDto> createValidator,
            IValidator<UpdatePatientDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            // Validar duplicados por nombre
            var existingByName = await _repository.GetByNameAsync(dto.Name);
            if (existingByName.Any())
                throw new InvalidOperationException($"Ya existe un paciente con el nombre '{dto.Name}'.");

            // Validar duplicados por identificación
            var existingByIdentification = await _repository.GetByIdentificationAsync(dto.Identification);
            if (existingByIdentification != null)
                throw new InvalidOperationException($"Ya existe un paciente con la identificación '{dto.Identification}'.");

            var patient = new Patient(
                Guid.NewGuid(),
                dto.Name,
                dto.Identification,
                dto.Age,
                dto.Contact,
                dto.Address
            );

            await _repository.AddAsync(patient);

            return _mapper.Map<PatientDto>(patient);
        }

        // -------------------------------
        // READ
        // -------------------------------
        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetByIdAsync(Guid id)
        {
            var patient = await _repository.GetByIdAsync(id);
            return _mapper.Map<PatientDto?>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetByNameAsync(string name)
        {
            var patients = await _repository.GetByNameAsync(name);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetByIdentificationAsync(string identification)
        {
            var patient = await _repository.GetByIdentificationAsync(identification);
            return _mapper.Map<PatientDto?>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetByAgeAsync(int age)
        {
            var patients = await _repository.GetByAgeAsync(age);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        /*public async Task<PatientDto?> GetWithRelationsAsync(Guid id)
        {
            var patient = await _repository.GetPatientWithAllRelationsAsync(id);
            return _mapper.Map<PatientDto?>(patient);
        }*/

        // -------------------------------
        // UPDATE
        // -------------------------------
        public async Task UpdateAsync(Guid id, UpdatePatientDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Paciente no encontrado.");

            // Evitar duplicado por nombre
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != patient.Name)
            {
                var existingByName = await _repository.GetByNameAsync(dto.Name);
                if (existingByName.Any(p => p.PatientId != id))
                    throw new InvalidOperationException($"Ya existe un paciente con el nombre '{dto.Name}'.");

                patient.ChangeName(dto.Name);
            }

            // Evitar duplicado por identificación
            if (!string.IsNullOrWhiteSpace(dto.Identification) && dto.Identification != patient.Identification)
            {
                var existingById = await _repository.GetByIdentificationAsync(dto.Identification);
                if (existingById != null && existingById.PatientId != id)
                    throw new InvalidOperationException($"Ya existe un paciente con la identificación '{dto.Identification}'.");

                patient.ChangeIdentification(dto.Identification);
            }

            // Actualizar otros campos
            if (dto.Age.HasValue) patient.ChangeAge(dto.Age.Value);
            if (!string.IsNullOrWhiteSpace(dto.Contact)) patient.ChangeContact(dto.Contact);
            if (!string.IsNullOrWhiteSpace(dto.Address)) patient.ChangeAddress(dto.Address);

            await _repository.UpdateAsync(patient);
        }

        // -------------------------------
        // DELETE
        // -------------------------------
        public async Task DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                throw new KeyNotFoundException("Paciente no encontrado.");

            await _repository.DeleteByIdAsync(id);
        }
    }
}
