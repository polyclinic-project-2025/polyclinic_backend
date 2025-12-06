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

public class NurseService :
    EmployeeService<Nurse, NurseResponse>,
    INurseService
{
    private readonly INurseRepository _nurseRepository;
    private readonly IValidator<CreateNurseRequest> _createValidator;
    private readonly IValidator<UpdateNurseRequest> _updateValidator;

    public NurseService(
        INurseRepository repository,
        IMapper mapper,
        IValidator<CreateNurseRequest> createValidator,
        IValidator<UpdateNurseRequest> updateValidator
    ) : base(repository, mapper)
    {
        _nurseRepository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<NurseResponse>> CreateAsync(CreateNurseRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if(!result.IsValid){
            return Result<NurseResponse>.Failure(result.Errors.First().ErrorMessage);
        }
        if(await _nurseRepository.ExistsByIdentificationAsync(request.Identification))
        {
            return Result<NurseResponse>.Failure("Ya existe un empleado con esta identificación.");
        }
        var nurse = new Nurse(
            Guid.NewGuid(),
            request.Identification,
            request.Name,
            request.EmploymentStatus
        );
        await _nurseRepository.AddAsync(nurse);
        var nurseResponse = _mapper.Map<NurseResponse>(nurse);
        return Result<NurseResponse>.Success(nurseResponse);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateNurseRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<bool>.Failure(result.Errors.First().ErrorMessage);
        }
        var nurse = await _nurseRepository.GetByIdAsync(id);
        if(nurse == null)
        {
            return Result<bool>.Failure("Enfermero no encontrado.");
        }
        if(!string.IsNullOrEmpty(request.Name))
        {
            nurse.UpdateName(request.Name);
        }
        if(!string.IsNullOrEmpty(request.Identification))
        {
            if(request.Identification != nurse.Identification 
                && await _nurseRepository.ExistsByIdentificationAsync(request.Identification))
            {
                return Result<bool>.Failure("Ya existe un enfermero con esta identificación.");
            }
            nurse.UpdateIdentification(request.Identification);
        }
        if(!string.IsNullOrEmpty(request.EmploymentStatus))
        {
            nurse.UpdateEmploymentStatus(request.EmploymentStatus);
        }

        await _nurseRepository.UpdateAsync(nurse);
        return Result<bool>.Success(true);
    }
}