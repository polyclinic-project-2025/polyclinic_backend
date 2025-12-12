using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using PolyclinicDomain.Entities;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations
{
    public class EmergencyRoomCareService : IEmergencyRoomCareService
    {
        private readonly IEmergencyRoomCareRepository _repository;
        private readonly IEmergencyRoomRepository _emergencyRoomRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmergencyRoomCareDto> _createValidator;
        private readonly IValidator<UpdateEmergencyRoomCareDto> _updateValidator;

        public EmergencyRoomCareService(
            IEmergencyRoomCareRepository repository,
            IEmergencyRoomRepository emergencyRoomRepository,
            IPatientRepository patientRepository,
            IMapper mapper,
            IValidator<CreateEmergencyRoomCareDto> createValidator,
            IValidator<UpdateEmergencyRoomCareDto> updateValidator)
        {
            _repository = repository;
            _emergencyRoomRepository = emergencyRoomRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // **************************************
        // CREATE
        // **************************************
        public async Task<Result<EmergencyRoomCareDto>> CreateAsync(CreateEmergencyRoomCareDto dto)
        {
            // Validación básica del DTO
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Result<EmergencyRoomCareDto>.Failure(validation.Errors.First().ErrorMessage);

            // Validaciones de existencia (en servicio)
            var emergencyRoom = await _emergencyRoomRepository.GetByIdAsync(dto.EmergencyRoomId);
            if (emergencyRoom == null)
                return Result<EmergencyRoomCareDto>.Failure("La sala de emergencia especificada no existe");

            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                return Result<EmergencyRoomCareDto>.Failure("El paciente especificado no existe");
            
            var doctorId = emergencyRoom.DoctorId;
            var careDate = DateOnly.FromDateTime(dto.CareDate);
            var isDoctorOnGuard = await _emergencyRoomRepository.IsDoctorOnGuardAsync(doctorId, careDate);
        
            if (!isDoctorOnGuard)
            {
                return Result<EmergencyRoomCareDto>.Failure(
                    $"Si no está de guardia en la fecha {careDate:dd/MM/yyyy} no puede realizar atenciones");
            }

            var entity = new EmergencyRoomCare(
                Guid.NewGuid(),
                dto.Diagnosis,
                dto.EmergencyRoomId,
                dto.CareDate,
                dto.PatientId
            );

            await _repository.AddAsync(entity);

            // Obtener la entidad con relaciones para el DTO
            var createdEntity = await _repository.GetByIdWithDetailsAsync(entity.EmergencyRoomCareId);
            var resultDto = _mapper.Map<EmergencyRoomCareDto>(createdEntity);
            
            return Result<EmergencyRoomCareDto>.Success(resultDto);
        }

        // **************************************
        // UPDATE
        // **************************************
        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateEmergencyRoomCareDto dto)
        {
            // Validación básica del DTO
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

            // Validar que la entidad exista
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure($"Atención de emergencia {id} no encontrada");

            // Obtener nuevos valores o mantener los existentes
            var diagnosis = dto.Diagnosis ?? entity.Diagnosis;
            var emergencyRoomId = dto.EmergencyRoomId ?? entity.EmergencyRoomId;
            var careDate = dto.CareDate ?? entity.CareDate;
            var patientId = dto.PatientId ?? entity.PatientId;

            // Validaciones de existencia y reglas de negocio
            if (dto.EmergencyRoomId.HasValue)
            {
                var emergencyRoom = await _emergencyRoomRepository.GetByIdAsync(emergencyRoomId);
                if (emergencyRoom == null)
                    return Result<bool>.Failure("La sala de emergencia especificada no existe");
            }

            if (dto.PatientId.HasValue)
            {
                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                    return Result<bool>.Failure("El paciente especificado no existe");
            }
            if (dto.EmergencyRoomId.HasValue || dto.CareDate.HasValue)
        {
            var emergencyRoom = await _emergencyRoomRepository.GetByIdAsync(emergencyRoomId);
            var doctorId = emergencyRoom.DoctorId;
            var careDateOnly = DateOnly.FromDateTime(careDate);
            
            var isDoctorOnGuard = await _emergencyRoomRepository.IsDoctorOnGuardAsync(doctorId, careDateOnly);
            
            if (!isDoctorOnGuard)
            {
                return Result<bool>.Failure(
                    $"Si no está de guardia en la fecha {careDateOnly:dd/MM/yyyy} no puede realizar atenciones");
            }
        }
            // ACTUALIZAR LA MISMA ENTIDAD TRACKADA
            entity.Diagnosis = diagnosis;
            entity.EmergencyRoomId = emergencyRoomId;
            entity.CareDate = careDate;
            entity.PatientId = patientId;

            // Guardar cambios
            await _repository.UpdateAsync(entity);

            return Result<bool>.Success(true);
        }

        // **************************************
        // DELETE
        // **************************************
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            // Validar que el ID no sea vacío
            if (id == Guid.Empty)
                return Result<bool>.Failure("El ID es requerido");

            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure("Atención de emergencia no encontrada");

            await _repository.DeleteAsync(entity);
            return Result<bool>.Success(true);
        }

        // **************************************
        // GET BY ID WITH DETAILS
        // **************************************
        public async Task<Result<EmergencyRoomCareDto>> GetByIdWithDetailsAsync(Guid id)
        {
            // Validar que el ID no sea vacío
            if (id == Guid.Empty)
                return Result<EmergencyRoomCareDto>.Failure("El ID es requerido");

            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity is null)
                return Result<EmergencyRoomCareDto>.Failure("Atención de emergencia no encontrada");

            var resultDto = _mapper.Map<EmergencyRoomCareDto>(entity);
            return Result<EmergencyRoomCareDto>.Success(resultDto);
        }

        // **************************************
        // GET ALL WITH DETAILS
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetAllWithDetailsAsync()
        {
            var entities = await _repository.GetAllWithDetailsAsync();
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DATE
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDateAsync(DateTime date)
        {
            // Validación simple en servicio
            if (date == default)
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("La fecha es requerida");

            var entities = await _repository.GetByDateAsync(date);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("Atención de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DOCTOR NAME
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDoctorNameAsync(string doctorName)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(doctorName))
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("El nombre del doctor es requerido");

            var entities = await _repository.GetByDoctorNameAsync(doctorName);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("Atención de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DOCTOR IDENTIFICATION
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDoctorIdentificationAsync(string doctorIdentification)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(doctorIdentification))
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("La identificación del doctor es requerida");

            var entities = await _repository.GetByDoctorIdentificationAsync(doctorIdentification);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("Atención de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }

        // **************************************
        // FILTER BY PATIENT NAME
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByPatientNameAsync(string patientName)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(patientName))
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("El nombre del paciente es requerido");

            var entities = await _repository.GetByPatientNameAsync(patientName);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("Atención de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }

        // **************************************
        // FILTER BY PATIENT IDENTIFICATION
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByPatientIdentificationAsync(string patientIdentification)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(patientIdentification))
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("La identificación del paciente es requerida");

            var entities = await _repository.GetByPatientIdentificationAsync(patientIdentification);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomCareDto>>.Failure("Atención de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomCareDto>>(entities);
            return Result<IEnumerable<EmergencyRoomCareDto>>.Success(list);
        }
    }
}