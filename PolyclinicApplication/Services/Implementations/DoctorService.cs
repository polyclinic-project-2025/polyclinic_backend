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

public class DoctorService : 
    EmployeeService<Doctor, DoctorResponse>,
    IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    
    public DoctorService(
        IDoctorRepository repository,
        IMapper mapper,
        IDepartmentRepository departmentRepository
    ) : base(repository, mapper)
    {
        _doctorRepository = repository;
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request)
    {
        try
        {
            if(await _doctorRepository.ExistsByIdentificationAsync(request.Identification))
            {
                return Result<DoctorResponse>.Failure("Ya existe un empleado con esta identificación.");
            }
            if(await _departmentRepository.GetByIdAsync(request.DepartmentId) == null)
            {
                return Result<DoctorResponse>.Failure("Departamento no encontrado.");
            }
            var doctor = new Doctor(
                Guid.NewGuid(),
                request.Identification,
                request.Name,
                request.EmploymentStatus,
                request.DepartmentId
            );
            
            await _doctorRepository.AddAsync(doctor);
            
            var doctorResponse = _mapper.Map<DoctorResponse>(doctor);
            return Result<DoctorResponse>.Success(doctorResponse);
        }
        catch (Exception ex)
        {
            return Result<DoctorResponse>.Failure($"Error al guardar el doctor: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateDoctorRequest request)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
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
                    && await _doctorRepository.ExistsByIdentificationAsync(request.Identification))
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
            
            await _doctorRepository.UpdateAsync(doctor);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar el doctor: {ex.Message}");
        }
    }
}