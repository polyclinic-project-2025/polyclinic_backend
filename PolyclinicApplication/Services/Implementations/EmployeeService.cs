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

public class EmployeeService<TEntity, TResponse> :
    IEmployeeService<TResponse>
    where TEntity : Employee
{
    private readonly IEmployeeRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    public EmployeeService(
        IEmployeeRepository<TEntity> repository,
        IMapper mapper
    )
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<Result<IEnumerable<TResponse>>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        var employeeResponse = _mapper.Map<IEnumerable<TResponse>>(employees);
        return Result<IEnumerable<TResponse>>.Success(employeeResponse);
    }

    public virtual async Task<Result<TResponse>> GetByIdAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if(employee == null)
        {
            return Result<TResponse>.Failure("Empleado no encontrado.");
        }
        var employeeResponse = _mapper.Map<TResponse>(employee);
        return Result<TResponse>.Success(employeeResponse);
    }

    public virtual async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if(employee == null)
        {
            return Result<bool>.Failure("Empleado no encontrado.");
        }
        await _repository.DeleteAsync(employee);
        return Result<bool>.Success(true);
    }
}