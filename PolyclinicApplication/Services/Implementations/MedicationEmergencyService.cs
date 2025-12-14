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
    public class MedicationEmergencyService : IMedicationEmergencyService
    {
        private readonly IMedicationEmergencyRepository _repository;
        private readonly IEmergencyRoomCareRepository _emergencyRoomCareRepository;
        private readonly IMedicationRepository _medicationRepository;
        private readonly IMapper _mapper;

        public MedicationEmergencyService(
            IMedicationEmergencyRepository repository,
            IEmergencyRoomCareRepository emergencyRoomCareRepository,
            IMedicationRepository medicationRepository,
            IMapper mapper)
        {
            _repository = repository;
            _emergencyRoomCareRepository = emergencyRoomCareRepository;
            _medicationRepository = medicationRepository;
            _mapper = mapper;
        }

        // **************************************
        // CREATE
        // **************************************
        public async Task<Result<MedicationEmergencyDto>> CreateAsync(CreateMedicationEmergencyDto dto)
        {
            try {
            // Validaciones de existencia (en servicio)
            var emergencyRoomCare = await _emergencyRoomCareRepository.GetByIdAsync(dto.EmergencyRoomCareId);
            if (emergencyRoomCare == null)
                return Result<MedicationEmergencyDto>.Failure("La atención de emergencia especificada no existe");

            var medication = await _medicationRepository.GetByIdAsync(dto.MedicationId);
            if (medication == null)
                return Result<MedicationEmergencyDto>.Failure("El medicamento especificado no existe");

            var entity = new MedicationEmergency(
                Guid.NewGuid(),
                dto.Quantity,
                dto.EmergencyRoomCareId,
                dto.MedicationId
            );

            await _repository.AddAsync(entity);

            // Obtener la entidad con relaciones para el DTO
            var createdEntity = await _repository.GetByIdWithMedicationAsync(entity.MedicationEmergencyId);
            var resultDto = _mapper.Map<MedicationEmergencyDto>(createdEntity);
            
            return Result<MedicationEmergencyDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<MedicationEmergencyDto>.Failure($"Error al guardar medicacion: {ex.Message}");
            }
        }

        // **************************************
        // UPDATE
        // **************************************
        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationEmergencyDto dto)
        {
            try{
            // Validar que la entidad exista
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure($"Medicación de emergencia no encontrada");

            // Obtener nuevos valores o mantener los existentes
            var quantity = dto.Quantity ?? entity.Quantity;
            var emergencyRoomCareId = dto.EmergencyRoomCareId ?? entity.EmergencyRoomCareId;
            var medicationId = dto.MedicationId ?? entity.MedicationId;

            // Validaciones de existencia
            if (dto.EmergencyRoomCareId.HasValue)
            {
                var emergencyRoomCare = await _emergencyRoomCareRepository.GetByIdAsync(emergencyRoomCareId);
                if (emergencyRoomCare == null)
                    return Result<bool>.Failure("La atención de emergencia especificada no existe");
            }

            if (dto.MedicationId.HasValue)
            {
                var medication = await _medicationRepository.GetByIdAsync(medicationId);
                if (medication == null)
                    return Result<bool>.Failure("El medicamento especificado no existe");
            }

            // ACTUALIZAR LA MISMA ENTIDAD TRACKADA
            entity.Quantity = quantity;
            entity.EmergencyRoomCareId = emergencyRoomCareId;
            entity.MedicationId = medicationId;

            // Guardar cambios
            await _repository.UpdateAsync(entity);

            return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al actualizar medicacion: {ex.Message}");
            }
        }

        // **************************************
        // DELETE
        // **************************************
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try {
            // Validar que el ID no sea vacío
            if (id == Guid.Empty)
                return Result<bool>.Failure("El ID es requerido");

            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure("Medicación de emergencia no encontrada");

            await _repository.DeleteAsync(entity);
            return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar la medicacion: {ex.Message}");
            }
        }

        // **************************************
        // GET BY ID WITH MEDICATION
        // **************************************
        public async Task<Result<MedicationEmergencyDto>> GetByIdWithMedicationAsync(Guid id)
        {
            try {
            // Validar que el ID no sea vacío
            if (id == Guid.Empty)
                return Result<MedicationEmergencyDto>.Failure("El ID es requerido");

            var entity = await _repository.GetByIdWithMedicationAsync(id);
            if (entity is null)
                return Result<MedicationEmergencyDto>.Failure("Medicación de emergencia no encontrada");

            var resultDto = _mapper.Map<MedicationEmergencyDto>(entity);
            return Result<MedicationEmergencyDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<MedicationEmergencyDto>.Failure($"Error al obtener medicamento: {ex.Message}");
            }
        }

        // **************************************
        // GET ALL WITH MEDICATION
        // **************************************
        public async Task<Result<IEnumerable<MedicationEmergencyDto>>> GetAllWithMedicationAsync()
        {
            try {
            var entities = await _repository.GetAllWithMedicationAsync();
            var list = _mapper.Map<IEnumerable<MedicationEmergencyDto>>(entities);
            return Result<IEnumerable<MedicationEmergencyDto>>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure($"Error al obtener medicamentos: {ex.Message}");
            }
        }

        // **************************************
        // FILTER BY EMERGENCY ROOM CARE ID
        // **************************************
        public async Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByEmergencyRoomCareIdAsync(Guid emergencyRoomCareId)
        {
            try{
            // Validar que el ID no sea vacío
            if (emergencyRoomCareId == Guid.Empty)
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("El ID de atención de emergencia es requerido");

            // Verificar que exista la atención
            var emergencyRoomCare = await _emergencyRoomCareRepository.GetByIdAsync(emergencyRoomCareId);
            if (emergencyRoomCare == null)
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("La atención de emergencia especificada no existe");

            var entities = await _repository.GetByEmergencyRoomCareIdAsync(emergencyRoomCareId);
            if(!entities.Any())
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("Medicaciones de emergencia no encontradas");
            var list = _mapper.Map<IEnumerable<MedicationEmergencyDto>>(entities);
            return Result<IEnumerable<MedicationEmergencyDto>>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure($"Error al obtener medicamentos: {ex.Message}");
            }
        }

        // **************************************
        // FILTER BY MEDICATION ID
        // **************************************
        public async Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByMedicationIdAsync(Guid medicationId)
        {
            try{
            // Validar que el ID no sea vacío
            if (medicationId == Guid.Empty)
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("El ID del medicamento es requerido");

            // Verificar que exista el medicamento
            var medication = await _medicationRepository.GetByIdAsync(medicationId);
            if (medication == null)
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("El medicamento especificado no existe");

            var entities = await _repository.GetByMedicationIdAsync(medicationId);
            if (!entities.Any())
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("Medicaciones de emergencia no encontradas");
            var list = _mapper.Map<IEnumerable<MedicationEmergencyDto>>(entities);
            return Result<IEnumerable<MedicationEmergencyDto>>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure($"Error al obtener medicamentos: {ex.Message}");
            }
        }

        // **************************************
        // FILTER BY MEDICATION NAME
        // **************************************
        public async Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByMedicationNameAsync(string medicationName)
        {
            try{
            // Validación simple en servicio
            if (string.IsNullOrWhiteSpace(medicationName))
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("El nombre del medicamento es requerido");

            var entities = await _repository.GetByMedicationNameAsync(medicationName);
            if (!entities.Any())
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure("Medicaciones de emergencia no encontradas");
            var list = _mapper.Map<IEnumerable<MedicationEmergencyDto>>(entities);
            return Result<IEnumerable<MedicationEmergencyDto>>.Success(list);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<MedicationEmergencyDto>>.Failure($"Error al obtener medicamentos: {ex.Message}");
            }
        }
    }
}