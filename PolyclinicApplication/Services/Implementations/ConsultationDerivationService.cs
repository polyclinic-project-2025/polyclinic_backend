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

namespace PolyclinicApplication.Services.Implementations;

public class ConsultationDerivationService : IConsultationDerivationService
{
    private readonly IConsultationDerivationRepository _repository;
    private readonly IDerivationRepository _derivationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentHeadRepository _departmentHeadRepository;

    private readonly IMapper _mapper;
    private readonly IValidator<CreateConsultationDerivationDto> _createValidator;
    private readonly IValidator<UpdateConsultationDerivationDto> _updateValidator;

    public ConsultationDerivationService(
        IConsultationDerivationRepository repository,
        IDerivationRepository derivationRepository,
        IDoctorRepository doctorRepository,
        IDepartmentHeadRepository departmentHeadRepository,
        IMapper mapper,
        IValidator<CreateConsultationDerivationDto> createValidator,
        IValidator<UpdateConsultationDerivationDto> updateValidator)
    {
        _repository = repository;
        _derivationRepository = derivationRepository;
        _doctorRepository = doctorRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // **************************************
    // CREATE
    // **************************************
    public async Task<ResultT<ConsultationDerivationDto>> CreateAsync(
        CreateConsultationDerivationDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ResultT<ConsultationDerivationDto>.Failure(validation.Errors);

        // Validate foreign keys
        var derivation = await _derivationRepository.GetByIdAsync(dto.DerivationId);
        if (derivation is null)
            return ResultT<ConsultationDerivationDto>.Failure("Derivation not found.");

        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor is null)
            return ResultT<ConsultationDerivationDto>.Failure("Doctor not found.");

        var deptHead = await _departmentHeadRepository.GetByIdAsync(dto.DepartmentHeadId);
        if (deptHead is null)
            return ResultT<ConsultationDerivationDto>.Failure("DepartmentHead not found.");

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
        return ResultT<ConsultationDerivationDto>.Success(resultDto);
    }

    // **************************************
    // UPDATE
    // **************************************
    public async Task<ResultT<ConsultationDerivationDto>> UpdateAsync(Guid id, UpdateConsultationDerivationDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ResultT<ConsultationDerivationDto>.Failure(validation.Errors);

        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return ResultT<ConsultationDerivationDto>.Failure("Record not found.");

        // Validación de claves foráneas
        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor is null)
            return ResultT<ConsultationDerivationDto>.Failure("Doctor not found.");

        var deptHead = await _departmentHeadRepository.GetByIdAsync(dto.DepartmentHeadId);
        if (deptHead is null)
            return ResultT<ConsultationDerivationDto>.Failure("DepartmentHead not found.");

        if (!string.IsNullOrWhiteSpace(dto.Diagnosis))
        entity.UpdateDiagnosis(dto.Diagnosis);

        if (dto.DateTimeCDer != default)
            entity.UpdateDateTimeCDer(dto.DateTimeCDer);

        if (dto.DoctorId != Guid.Empty)
            entity.UpdateDoctorId(dto.DoctorId);

        if (dto.DepartmentHeadId != Guid.Empty)
            entity.UpdateDepartmentHeadId(dto.DepartmentHeadId);

        await _repository.UpdateAsync(entity);

        return ResultT<bool>.Success(true);
    }

    // **************************************
    // DELETE
    // **************************************
    public async Task<ResultT<bool>> DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return ResultT<bool>.Failure("Record not found.");

        await _repository.DeleteAsync(entity);
        return ResultT<bool>.Success(true);
    }

    // **************************************
    // GET BY ID
    // **************************************
    public async Task<ResultT<ConsultationDerivationDto>> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            return ResultT<ConsultationDerivationDto>.Failure("Record not found.");

        var resultDto = _mapper.Map<ConsultationDerivationDto>(entity);
        return ResultT<ConsultationDerivationDto>.Success(resultDto);
    }

    // **************************************
    // GET ALL
    // **************************************
    public async Task<ResultT<IEnumerable<ConsultationDerivationDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        var list = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(entities);
        return ResultT<IEnumerable<ConsultationDerivationDto>>.Success(list);
    }

    // **************************************
    // CUSTOM: Get by Date Range
    // **************************************
    public async Task<ResultT<IEnumerable<ConsultationDerivationDto>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate)
    {
        var records = await _repository.GetByDateRangeAsync(patientId, startDate, endDate);
        var result = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(records);
        return ResultT<IEnumerable<ConsultationDerivationDto>>.Success(result);
    }

    // **************************************
    // CUSTOM: Last 10
    // **************************************
    public async Task<ResultT<IEnumerable<ConsultationDerivationDto>>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        var records = await _repository.GetLast10ByPatientIdAsync(patientId);
        var result = _mapper.Map<IEnumerable<ConsultationDerivationDto>>(records);
        return ResultT<IEnumerable<ConsultationDerivationDto>>.Success(result);
    }
}