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
using PolyclinicApplication.Common.Results;

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
        public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);
            if(!result.IsValid)
            {
                return Result<PatientDto>.Failure(result.Errors.First().ErrorMessage);
            }
            // Validar duplicados por identificación
            var existingByIdentification = await _repository.GetByIdentificationAsync(dto.Identification);
            if (existingByIdentification != null)
                return Result<PatientDto>.Failure("Ya existe un paciente con esta identificación.");

            var patient = new Patient(
                Guid.NewGuid(),
                dto.Name,
                dto.Identification,
                dto.Age,
                dto.Contact,
                dto.Address
            );

            try
            {
                await _repository.AddAsync(patient);
            }
            catch (Exception ex)
            {
                return Result<PatientDto>.Failure($"Error al guardar el paciente: {ex.Message}");
            }

            var patientdto = _mapper.Map<PatientDto>(patient);
            return Result<PatientDto>.Success(patientdto);
        }

        // -------------------------------
        // READ
        // -------------------------------
        public async Task<Result<IEnumerable<PatientDto>>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();
            var patientsdto = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return Result<IEnumerable<PatientDto>>.Success(patientsdto);
        }

        public async Task<Result<PatientDto>> GetByIdAsync(Guid id)
        {
            var patient = await _repository.GetByIdAsync(id);
            if(patient == null)
            {
                return Result<PatientDto>.Failure("Paciente no encontrado.");
            }
            var patientdto = _mapper.Map<PatientDto>(patient);
            return Result<PatientDto>.Success(patientdto);
        }

        public async Task<Result<IEnumerable<PatientDto>>> GetByNameAsync(string name)
        {
            var patients = await _repository.GetByNameAsync(name);
            if(!patients.Any())
            {
                return Result<IEnumerable<PatientDto>>.Failure("Paciente no encontrado.");
            }
            var patientsdto = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return Result<IEnumerable<PatientDto>>.Success(patientsdto);
        }

        public async Task<Result<PatientDto>> GetByIdentificationAsync(string identification)
        {
            var patient = await _repository.GetByIdentificationAsync(identification);
            if(patient == null)
            {
                return Result<PatientDto>.Failure("Paciente no encontrado.");
            }
            var patientdto = _mapper.Map<PatientDto>(patient);
            return Result<PatientDto>.Success(patientdto);
        }

        public async Task<Result<IEnumerable<PatientDto>>> GetByAgeAsync(int age)
        {
            var patients = await _repository.GetByAgeAsync(age);
            if(!patients.Any())
            {
                return Result<IEnumerable<PatientDto>>.Failure("Paciente no encontrado.");
            }
            var patientsdto =_mapper.Map<IEnumerable<PatientDto>>(patients);
            return Result<IEnumerable<PatientDto>>.Success(patientsdto);
        }

        // -------------------------------
        // UPDATE
        // -------------------------------
        public async Task<Result<bool>> UpdateAsync(Guid id, UpdatePatientDto dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);
            if(!result.IsValid)
            {
                return Result<bool>.Failure(result.Errors.First().ErrorMessage);
            }
            var patient = await _repository.GetByIdAsync(id);
            if (patient == null)
                return Result<bool>.Failure("Paciente no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                patient.ChangeName(dto.Name);
            }

            // Evitar duplicado por identificación
            if (!string.IsNullOrWhiteSpace(dto.Identification))
            {
                var existingById = await _repository.GetByIdentificationAsync(dto.Identification);
                if (existingById != null && existingById.Identification != patient.Identification)
                    return Result<bool>.Failure("Ya existe un paciente con esta identificación.");

                patient.ChangeIdentification(dto.Identification);
            }

            // Actualizar otros campos
            if (dto.Age != null) patient.ChangeAge(dto.Age);
            if (!string.IsNullOrWhiteSpace(dto.Contact)) patient.ChangeContact(dto.Contact);
            if (!string.IsNullOrWhiteSpace(dto.Address)) patient.ChangeAddress(dto.Address);

            try
            {
                await _repository.UpdateAsync(patient);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al actualizar el paciente: {ex.Message}");
            }
        }

        // -------------------------------
        // DELETE
        // -------------------------------
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
                return Result<bool>.Failure("Paciente no encontrado.");

            try
            {
                await _repository.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar el paciente: {ex.Message}");
            }
        }
    }
}
