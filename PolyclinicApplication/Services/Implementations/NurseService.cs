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

public class NurseService :
    EmployeeService<Nurse, NurseResponse>,
    INurseService
{
    private readonly INurseRepository _nurseRepository;

    public NurseService(
        INurseRepository repository,
        IMapper mapper
    ) : base(repository, mapper)
    {
        _nurseRepository = repository;
    }

    public async Task<Result<NurseResponse>> CreateAsync(CreateNurseRequest request)
    {
        try
        {
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
        catch (Exception ex)
        {
            return Result<NurseResponse>.Failure($"Error al guardar el enfermero: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateNurseRequest request)
    {
        try
        {
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
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar el enfermero: {ex.Message}");
        }
    }
}