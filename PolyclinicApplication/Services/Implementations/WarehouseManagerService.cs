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

public class WarehouseManagerService :
    EmployeeService<WarehouseManager, WarehouseManagerResponse>,
    IWarehouseManagerService
{
    private readonly IWarehouseManagerRepository _warehouseManagerRepository;
    private readonly IValidator<CreateWarehouseManagerRequest> _createValidator;
    private readonly IValidator<UpdateWarehouseManagerRequest> _updateValidator;

    public WarehouseManagerService(
        IWarehouseManagerRepository repository,
        IMapper mapper,
        IValidator<CreateWarehouseManagerRequest> createValidator,
        IValidator<UpdateWarehouseManagerRequest> updateValidator
    ) : base(repository, mapper)
    {
        _warehouseManagerRepository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<WarehouseManagerResponse>> GetWarehouseManagerAsync()
    {
        var warehouseManager = await _warehouseManagerRepository.GetAsync();
        if (warehouseManager == null)
        {
            return Result<WarehouseManagerResponse>.Failure("Jefe de almacén no encontrado.");
        }
        var response = _mapper.Map<WarehouseManagerResponse>(warehouseManager);
        return Result<WarehouseManagerResponse>.Success(response);
    }

    public async Task<Result<WarehouseManagerResponse>> CreateAsync(CreateWarehouseManagerRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if(!result.IsValid){
            return Result<WarehouseManagerResponse>.Failure(result.Errors.First().ErrorMessage);
        }
        if(await _warehouseManagerRepository.ExistsByIdentificationAsync(request.Identification))
        {
            return Result<WarehouseManagerResponse>.Failure("Ya existe un empleado con esta identificación.");
        }
        var warehouseManager = new WarehouseManager(
            Guid.NewGuid(),
            request.Identification,
            request.Name,
            request.EmploymentStatus,
            DateTime.UtcNow
        );
        await _warehouseManagerRepository.AddAsync(warehouseManager);
        var warehouseManagerResponse = _mapper.Map<WarehouseManagerResponse>(warehouseManager);
        return Result<WarehouseManagerResponse>.Success(warehouseManagerResponse);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateWarehouseManagerRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<bool>.Failure(result.Errors.First().ErrorMessage);
        }
        var warehouseManager = await _warehouseManagerRepository.GetByIdAsync(id);
        if(warehouseManager == null)
        {
            return Result<bool>.Failure("Jefe de almacén no encontrado.");
        }
        if(!string.IsNullOrEmpty(request.Name))
        {
            warehouseManager.UpdateName(request.Name);
        }
        if(!string.IsNullOrEmpty(request.Identification))
        {
            if(request.Identification != warehouseManager.Identification 
                && await _warehouseManagerRepository.ExistsByIdentificationAsync(request.Identification))
            {
                return Result<bool>.Failure("Ya existe un jefe de almacén con esta identificación.");
            }
            warehouseManager.UpdateIdentification(request.Identification);
        }
        if(!string.IsNullOrEmpty(request.EmploymentStatus))
        {
            warehouseManager.UpdateEmploymentStatus(request.EmploymentStatus);
        }
        await _warehouseManagerRepository.UpdateAsync(warehouseManager);
        return Result<bool>.Success(true);
    }
}