using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Implementations;

public class DepartmentHeadService : IDepartmentHeadService
{
    private readonly IDepartmentHeadRepository _repository;
    private readonly IMapper _mapper;

    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDoctorRepository _doctorRepository;

    public DepartmentHeadService(
        IDepartmentHeadRepository repository,
        IMapper mapper,
        IDepartmentRepository departmentRepository,
        IDoctorRepository doctorRepository
    )
    {
        _repository = repository;
        _mapper = mapper;
        _departmentRepository = departmentRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<Result<IEnumerable<DepartmentHeadResponse>>> GetAllDepartmentHeadAsync()
    {
        var departmentHeads = await _repository.GetAllAsync();
        var departmentHeadResponses = _mapper.Map<IEnumerable<DepartmentHeadResponse>>(departmentHeads);
        return Result<IEnumerable<DepartmentHeadResponse>>.Success(departmentHeadResponses);
    }

    public async Task<Result<DepartmentHeadResponse>> GetDepartmentHeadByIdAsync(Guid id)
    {
        var departmentHead = await _repository.GetByIdAsync(id);
        if (departmentHead == null)
        {
            return Result<DepartmentHeadResponse>.Failure("Jefe de departamento no encontrado.");
        }
        var response = _mapper.Map<DepartmentHeadResponse>(departmentHead);
        return Result<DepartmentHeadResponse>.Success(response);
    }

    public async Task<Result<DepartmentHeadResponse>> GetDepartmentHeadByDepartmentIdAsync(Guid departmentId)
    {
        if(await _departmentRepository.GetByIdAsync(departmentId) == null)
        {
            return Result<DepartmentHeadResponse>.Failure("Departamento no encontrado.");
        }
        var departmentHead = await _repository.GetByDepartmentIdAsync(departmentId);
        if (departmentHead == null)
        {
            return Result<DepartmentHeadResponse>.Failure("Jefe de departamento no encontrado.");
        }
        var response = _mapper.Map<DepartmentHeadResponse>(departmentHead);
        return Result<DepartmentHeadResponse>.Success(response);
    }

    public async Task<Result<DepartmentHeadResponse>> AssignDepartmentHeadAsync(AssignDepartmentHeadRequest request)
    {
        if(await _doctorRepository.GetByIdAsync(request.DoctorId) == null)
        {
            return Result<DepartmentHeadResponse>.Failure("Doctor no encontrado.");
        }
        if(await _repository.GetByIdAsync(request.DoctorId) != null)
        {
            return Result<DepartmentHeadResponse>.Failure("El doctor ya es jefe de departamento.");
        }
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        var departmentHead = new DepartmentHead(Guid.NewGuid(), request.DoctorId, doctor.DepartmentId, DateTime.UtcNow);
        await _repository.AddAsync(departmentHead);
        var response = _mapper.Map<DepartmentHeadResponse>(departmentHead);
        return Result<DepartmentHeadResponse>.Success(response);
    }

    public async Task<Result<bool>> RemoveDepartmentHeadAsync(Guid departmentHeadId)
    {
        var departmentHead = await _repository.GetByIdAsync(departmentHeadId);
        if (departmentHead == null)
        {
            return Result<bool>.Failure("Jefe de departamento no encontrado.");
        }
        await _repository.DeleteAsync(departmentHead);
        return Result<bool>.Success(true);
    }
}