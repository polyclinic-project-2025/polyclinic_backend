using AutoMapper;
using FluentValidation;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Implementations
{
    public class EmergencyRoomService : IEmergencyRoomService
    {
        private readonly IEmergencyRoomRepository _repository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmergencyRoomDto> _createValidator;
        private readonly IValidator<UpdateEmergencyRoomDto> _updateValidator;

        public EmergencyRoomService(
            IEmergencyRoomRepository repository,
            IDoctorRepository doctorRepository,
            IMapper mapper,
            IValidator<CreateEmergencyRoomDto> createValidator,
            IValidator<UpdateEmergencyRoomDto> updateValidator)
        {
            _repository = repository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // **************************************
        // CREATE
        // **************************************
        public async Task<Result<EmergencyRoomDto>> CreateAsync(CreateEmergencyRoomDto dto)
        {
            // Validación básica del DTO
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Result<EmergencyRoomDto>.Failure(validation.Errors.First().ErrorMessage);

            // Validación de existencia (en servicio)
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null)
                return Result<EmergencyRoomDto>.Failure("El doctor especificado no existe");

            var entity = new EmergencyRoom(
                Guid.NewGuid(),
                dto.DoctorId,
                dto.GuardDate
            );

            await _repository.AddAsync(entity);

            // Obtener la entidad con relaciones para el DTO
            var createdEntity = await _repository.GetByIdWithDoctorAsync(entity.EmergencyRoomId);
            var resultDto = _mapper.Map<EmergencyRoomDto>(createdEntity);
            
            return Result<EmergencyRoomDto>.Success(resultDto);
        }

        // **************************************
        // UPDATE
        // **************************************
        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateEmergencyRoomDto dto)
        {

            // Validación básica del DTO
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

            // Validar que la entidad exista (en servicio)
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure($"Sala de emergencia {id} no encontrada");

            // Obtener nuevos valores o mantener los existentes
            var doctorId = dto.DoctorId ?? entity.DoctorId;
            var guardDate = dto.GuardDate ?? entity.GuardDate;

            // Validaciones de existencia y reglas de negocio (en servicio)
            if (dto.DoctorId.HasValue)
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                    return Result<bool>.Failure("El doctor especificado no existe");
            }

            // ACTUALIZAR LA MISMA ENTIDAD TRACKADA
            entity.DoctorId = doctorId;
            entity.GuardDate = guardDate;

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
                return Result<bool>.Failure("Sala de emergencia no encontrada");

            await _repository.DeleteAsync(entity);
            return Result<bool>.Success(true);
        }

        // **************************************
        // GET BY ID WITH DOCTOR
        // **************************************
        public async Task<Result<EmergencyRoomDto>> GetByIdWithDoctorAsync(Guid id)
        {
            // Validar que el ID no sea vacío
            if (id == Guid.Empty)
                return Result<EmergencyRoomDto>.Failure("El ID es requerido");

            var entity = await _repository.GetByIdWithDoctorAsync(id);
            if (entity is null)
                return Result<EmergencyRoomDto>.Failure("Sala de emergencia no encontrada");

            var resultDto = _mapper.Map<EmergencyRoomDto>(entity);
            return Result<EmergencyRoomDto>.Success(resultDto);
        }

        // **************************************
        // GET ALL WITH DOCTOR
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomDto>>> GetAllWithDoctorAsync()
        {
            var entities = await _repository.GetAllWithDoctorAsync();
            var list = _mapper.Map<IEnumerable<EmergencyRoomDto>>(entities);
            return Result<IEnumerable<EmergencyRoomDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DATE
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDateAsync(DateOnly date)
        {
            // Validación simple en servicio
            if (date == default)
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("La fecha es requerida");

            var entities = await _repository.GetByDateAsync(date);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("Sala de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomDto>>(entities);
            return Result<IEnumerable<EmergencyRoomDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DOCTOR IDENTIFICATION
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDoctorIdentificationAsync(string doctorIdentification)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(doctorIdentification))
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("La identificación del doctor es requerida");

            var entities = await _repository.GetByDoctorIdentificationAsync(doctorIdentification);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("Sala de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomDto>>(entities);
            return Result<IEnumerable<EmergencyRoomDto>>.Success(list);
        }

        // **************************************
        // FILTER BY DOCTOR NAME
        // **************************************
        public async Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDoctorNameAsync(string doctorName)
        {
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(doctorName))
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("El nombre del doctor es requerido");

            var entities = await _repository.GetByDoctorNameAsync(doctorName);
            if (!entities.Any())
                return Result<IEnumerable<EmergencyRoomDto>>.Failure("Sala de emergencia no encontrada");
            var list = _mapper.Map<IEnumerable<EmergencyRoomDto>>(entities);
            return Result<IEnumerable<EmergencyRoomDto>>.Success(list);
        }
    }
}