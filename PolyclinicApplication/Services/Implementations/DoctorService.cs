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

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDoctorRequest> _createValidator;
    private readonly IValidator<UpdateDoctorRequest> _updateValidator;

    private readonly IDepartmentRepository _departmentRepository;
    
    public DoctorService(
        IDoctorRepository repository,
        IMapper mapper,
        IValidator<CreateDoctorRequest> createValidator,
        IValidator<UpdateDoctorRequest> updateValidator,
        IDepartmentRepository departmentRepository
    )
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<IEnumerable<DoctorResponse>>> GetAllAsync()
    {
        var doctors = await _repository.GetAllAsync();
        var doctorResponse = _mapper.Map<IEnumerable<DoctorResponse>>(doctors);
        return Result<IEnumerable<DoctorResponse>>.Success(doctorResponse);
    }

    public async Task<Result<DoctorResponse>> GetByIdAsync(Guid id)
    {
        var doctor = await _repository.GetByIdAsync(id);
        if(doctor == null)
        {
            return Result<DoctorResponse>.Failure("Doctor no encontrado.");
        }
        var doctorResponse = _mapper.Map<DoctorResponse>(doctor);
        return Result<DoctorResponse?>.Success(doctorResponse);
    }

    public async Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<DoctorResponse>.Failure(result.Errors.First().ErrorMessage);
        }
        if(await _repository.ExistsByIdentificationAsync(request.Identification))
        {
            return Result<DoctorResponse>.Failure("Ya existe un empleado con esta identificación.");
        }
        if(await _departmentRepository.GetByIdAsync(request.DepartmentId) == null)
        {
            return Result<DoctorResponse>.Failure("Departamento no encontrado.");
        }
        var doctor = new Doctor(
            Guid.NewGuid(),
            request.Name,
            request.Identification,
            request.EmploymentStatus,
            request.DepartmentId
        );
        await _repository.AddAsync(doctor);
        var doctorResponse = _mapper.Map<DoctorResponse>(doctor);
        return Result<DoctorResponse>.Success(doctorResponse);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateDoctorRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<bool>.Failure(result.Errors.First().ErrorMessage);
        }
        var doctor = await _repository.GetByIdAsync(id);
        if(doctor == null)
        {
            return Result<bool>.Failure("Doctor no encontrado.");
        }
        if(!string.IsNullOrEmpty(request.Name))
        {
            doctor.UpdateName(request.Name);
        }
        if(!string.IsNullOrEmpty(request.Identification))
        {
            if(request.Identification != doctor.Identification 
                && await _repository.ExistsByIdentificationAsync(request.Identification))
            {
                return Result<bool>.Failure("Ya existe un doctor con esta identificación.");
            }
            doctor.UpdateIdentification(request.Identification);
        }
        if(!string.IsNullOrEmpty(request.EmploymentStatus))
        {
            doctor.UpdateEmploymentStatus(request.EmploymentStatus);
        }
        if(request.DepartmentId != null)
        {
            doctor.UpdateDepartmentId(request.DepartmentId.Value);
        }
        await _repository.UpdateAsync(doctor);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var doctor = await _repository.GetByIdAsync(id);
        if(doctor == null)
        {
            return Result<bool>.Failure("Doctor no encontrado.");
        }
        await _repository.DeleteAsync(doctor);
        return Result<bool>.Success(true);
    }
}