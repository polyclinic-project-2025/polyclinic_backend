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

public class WarehouseRequestService : IWarehouseRequestService
{
    private readonly IWarehouseRequestRepository _repository;
    private readonly IMapper _mapper;

    private readonly IWarehouseManagerRepository _warehouseManagerRepository;

    public WarehouseRequestService(
        IWarehouseRequestRepository repository,
        IMapper mapper,
        IWarehouseManagerRepository warehouseManagerRepository
    )
    {
        _repository = repository;
        _mapper = mapper;
        _warehouseManagerRepository = warehouseManagerRepository;
    }

    public async Task<Result<IEnumerable<WarehouseRequestResponse>>> GetAllWarehouseRequestAsync()
    {
        var warehouseRequests = await _repository.GetAllAsync();
        var responses = _mapper.Map<IEnumerable<WarehouseRequestResponse>>(warehouseRequests);
        return Result<IEnumerable<WarehouseRequestResponse>>.Success(responses);
    }

    public async Task<Result<WarehouseRequestResponse>> GetWarehouseRequestByIdAsync(Guid id)
    {
        var warehouseRequest = await _repository.GetByIdAsync(id);
        if(warehouseRequest == null)
        {
            return Result<WarehouseRequestResponse>.Failure("Solicitud al almacén no encontrada.");
        }
        var response = _mapper.Map<WarehouseRequestResponse>(warehouseRequest);
        return Result<WarehouseRequestResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAsync(string status)
    {
        var warehouseRequest = await _repository.GetByStatusAsync(status);
        if(warehouseRequest == null)
        {
            return Result<IEnumerable<WarehouseRequestResponse>>.Failure("Solicitud al almacén no encontrada.");
        }
        var response = _mapper.Map<IEnumerable<WarehouseRequestResponse>>(warehouseRequest);
        return Result<IEnumerable<WarehouseRequestResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByDepartmentIdAsync(Guid departmentId)
    {
        var warehouseRequest = await _repository.GetByDepartmentIdAsync(departmentId);
        if(warehouseRequest == null)
        {
            return Result<IEnumerable<WarehouseRequestResponse>>.Failure("Solicitud al almacén no encontrada.");
        }
        var response = _mapper.Map<IEnumerable<WarehouseRequestResponse>>(warehouseRequest);
        return Result<IEnumerable<WarehouseRequestResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAndDepartmentIdAsync(string status, Guid departmentId)
    {
        var warehouseRequest = await _repository.GetByStatusAndDepartmentIdAsync(status, departmentId);
        if(warehouseRequest == null)
        {
            return Result<IEnumerable<WarehouseRequestResponse>>.Failure("Solicitud al almacén no encontrada.");
        }
        var response = _mapper.Map<IEnumerable<WarehouseRequestResponse>>(warehouseRequest);
        return Result<IEnumerable<WarehouseRequestResponse>>.Success(response);
    }

    public async Task<Result<WarehouseRequestResponse>> CreateWarehouseRequestAsync(CreateWarehouseRequestRequest request)
    {
        var warehouseManager = await _warehouseManagerRepository.GetAsync();
        if(warehouseManager == null)
        {
            return Result<WarehouseRequestResponse>.Failure("Jefe de almacén no encontrado.");
        }
        var warehouseRequest = new WarehouseRequest(
            Guid.NewGuid(),
            "1",
            DateTime.UtcNow,
            request.DepartmentId,
            warehouseManager.GetEmployeeId()
        );

        try
        {
            await _repository.AddAsync(warehouseRequest);
        }
        catch (Exception ex)
        {
            return Result<WarehouseRequestResponse>.Failure($"Error al guardar la solicitud al almacén: {ex.Message}");
        }
        var response = _mapper.Map<WarehouseRequestResponse>(warehouseRequest);
        return Result<WarehouseRequestResponse>.Success(response);
    }

    public async Task<Result<bool>> UpdateWarehouseRequestAsync(Guid id, UpdateWarehouseRequestRequest request)
    {
        var warehouseRequest = await _repository.GetByIdAsync(id);
        if(warehouseRequest == null)
        {
            return Result<bool>.Failure("Solicitud al almacén no encontrada.");
        }
        if(!string.IsNullOrEmpty(request.Status)){
            warehouseRequest.UpdateStatus(request.Status);
        }

        try
        {
            await _repository.UpdateAsync(warehouseRequest);
            var response = _mapper.Map<WarehouseRequestResponse>(warehouseRequest);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar la solicitud al almacén: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteWarehouseRequestAsync(Guid id)
    {
        var warehouseRequest = await _repository.GetByIdAsync(id);
        if(id == null)
        {
            return Result<bool>.Failure("Solicitud al almacén no encontrada.");
        }

        try
        {
            await _repository.DeleteAsync(warehouseRequest);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la solicitud al almacén: {ex.Message}");
        }
    }
}