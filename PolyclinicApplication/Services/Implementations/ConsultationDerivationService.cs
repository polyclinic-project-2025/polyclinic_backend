using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class ConsultationDerivationService : IConsultationDerivationService
{
    private readonly IConsultationDerivationRepository _repository;
    private readonly IDerivationRepository _derivationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentHeadRepository _departmentHeadRepository;

    private readonly IMapper _mapper;

    public ConsultationDerivationService(
        IConsultationDerivationRepository repository,
        IDerivationRepository derivationRepository,
        IDoctorRepository doctorRepository,
        IDepartmentHeadRepository departmentHeadRepository,
        IMapper mapper)
    {
        _repository = repository;
        _derivationRepository = derivationRepository;
        _doctorRepository = doctorRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _mapper = mapper;
    }

    // **************************************
    // CREATE
    // **************************************
    public async Task<Result<ConsultationDerivationDto>> CreateAsync(
        CreateConsultationDerivationDto dto)
    {
        try
        {
            // Validate foreign keys
            var derivation = await _derivationRepository.GetByIdAsync(dto.DerivationId);
            if (derivation is null)
                return Result<ConsultationDerivationDto>.Failure("Derivation not found.");

            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor is null)
                return Result<ConsultationDerivationDto>.Failure("Doctor not found.");

            var deptHead = await _departmentHeadRepository.GetByIdAsync(dto.DepartmentHeadId);
            if (deptHead is null)
                return Result<ConsultationDerivationDto>.Failure("DepartmentHead not found.");

            // Validar que el DepartmentHead pertenezca al departamento destino de la derivación
            if (deptHead.DepartmentId != derivation.DepartmentToId)
                return Result<ConsultationDerivationDto>.Failure(
                    "El jefe de departamento debe pertenecer al mismo departamento destino de la derivación.");

            // Validar que el Doctor pertenezca al departamento destino de la derivación
            if (doctor.DepartmentId != derivation.DepartmentToId)
                return Result<ConsultationDerivationDto>.Failure(
                    "El doctor tratante debe pertenecer al mismo departamento destino de la derivación.");

            var entity = new ConsultationDerivation(
                Guid.NewGuid(),
                dto.Diagnosis,
                dto.DerivationId,
                dto.DateTimeCDer,
                dto.DoctorId,
                dto.DepartmentHeadId
            );

            await _repository.AddAsync(entity);

            var resultDto = _mapper.Map<ConsultationDerivationDto>(entity);
            return Result<ConsultationDerivationDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            return Result<ConsultationDerivationDto>.Failure($"Error al guardar la consulta");
        }
    }

    // **************************************
    // UPDATE
    // **************************************
    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateConsultationDerivationDto dto)
    {
        try
        {
            var consultation = await _repository.GetByIdAsync(id);
            if (consultation is null)
                return Result<bool>.Failure("Record not found.");

            // Obtener la derivación para validaciones
            var derivation = await _derivationRepository.GetByIdAsync(consultation.DerivationId);
            if (derivation is null)
                return Result<bool>.Failure("Derivation not found.");

            // Validación de claves foráneas
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor is null)
                return Result<bool>.Failure("Doctor not found.");

            var deptHead = await _departmentHeadRepository.GetByIdAsync(dto.DepartmentHeadId);
            if (deptHead is null)
                return Result<bool>.Failure("DepartmentHead not found.");

            // Validar que el DepartmentHead pertenezca al departamento destino de la derivación
            if (deptHead.DepartmentId != derivation.DepartmentToId)
                return Result<bool>.Failure(
                    "El jefe de departamento debe pertenecer al mismo departamento destino de la derivación.");

            // Validar que el Doctor pertenezca al departamento destino de la derivación
            if (doctor.DepartmentId != derivation.DepartmentToId)
                return Result<bool>.Failure(
                    "El doctor tratante debe pertenecer al mismo departamento destino de la derivación.");

            if (!string.IsNullOrWhiteSpace(dto.Diagnosis))
                consultation.UpdateDiagnosis(dto.Diagnosis);

            if (dto.DateTimeCDer != default)
                consultation.UpdateDateTimeCDer(dto.DateTimeCDer);

            if (dto.DoctorId != Guid.Empty)
                consultation.UpdateDoctorId(dto.DoctorId);

            if (dto.DepartmentHeadId != Guid.Empty)
                consultation.UpdateDepartmentHeadId(dto.DepartmentHeadId);

            await _repository.UpdateAsync(consultation);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar la consulta");
        }
    }

    // **************************************
    // DELETE
    // **************************************
    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<bool>.Failure("Record not found.");

            await _repository.DeleteAsync(entity);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la consulta");
        }
    }

    // **************************************
    // GET BY ID
    // **************************************
    public async Task<Result<ConsultationDerivationDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Result<ConsultationDerivationDto>.Failure("Record not found.");

            var resultDto = _mapper.Map<ConsultationDerivationDto>(entity);
            return Result<ConsultationDerivationDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            return Result<ConsultationDerivationDto>.Failure($"Error al obtener consulta");
        }
    }

    // **************************************
    // GET ALL
    // **************************************
    public async Task<Result<IEnumerable<ConsultationDerivationDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var list = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(entities);
            return Result<IEnumerable<ConsultationDerivationDto>>.Success(list);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ConsultationDerivationDto>>.Failure($"Error al obtener consultas");
        }
    }

    // **************************************
    // CUSTOM: Get by Date Range
    // **************************************
    public async Task<Result<IEnumerable<ConsultationDerivationDto>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var records = await _repository.GetByDateRangeAsync(patientId, startDate, endDate);
            var result = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(records);
            return Result<IEnumerable<ConsultationDerivationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ConsultationDerivationDto>>.Failure($"Error al obtener consultas");
        }
    }

    // **************************************
    // CUSTOM: Last 10
    // **************************************
    public async Task<Result<IEnumerable<ConsultationDerivationDto>>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        try
        {
            var records = await _repository.GetLast10ByPatientIdAsync(patientId);
            var result = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(records);
            return Result<IEnumerable<ConsultationDerivationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ConsultationDerivationDto>>.Failure($"Error al obtener consultas");
        }
    }
}