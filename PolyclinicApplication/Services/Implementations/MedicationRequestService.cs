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
public class MedicationRequestService : IMedicationRequestService
{
    private readonly IMedicationRequestRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMedicationRequestRequest> _createValidator;
    private readonly IValidator<UpdateMedicationRequestRequest> _updateValidator;

    public MedicationRequestService(
        IMedicationRequestRepository repository,
        IMapper mapper,
        IValidator<CreateMedicationRequestRequest> createValidator,
        IValidator<UpdateMedicationRequestRequest> updateValidator
    )
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<IEnumerable<MedicationRequestResponse>>> GetAllMedicationRequestAsync()
    {
        var medicationRequest = await _repository.GetAllAsync();
        var responses = _mapper.Map<IEnumerable<MedicationRequestResponse>>(medicationRequest);
        return Result<IEnumerable<MedicationRequestResponse>>.Success(responses);
    }

    public async Task<Result<MedicationRequestResponse>> GetMedicationRequestByIdAsync(Guid id)
    {
        var medicationRequest = await _repository.GetByIdAsync(id);
        if(medicationRequest == null)
        {
            return Result<MedicationRequestResponse>.Failure("Solicitud de medicamentos al almacén no encontrada.");
        }
        var response = _mapper.Map<MedicationRequestResponse>(medicationRequest);
        return Result<MedicationRequestResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationRequestResponse>>> GetMedicationRequestByWarehouseRequestIdAsync(Guid warehouseRequestId)
    {
        var medicationRequest = await _repository.GetByWarehouseRequestIdAsync(warehouseRequestId);
        if(medicationRequest == null)
        {
            return Result<IEnumerable<MedicationRequestResponse>>.Failure("Solicitud al almacén no encontrada.");
        }
        var response = _mapper.Map<IEnumerable<MedicationRequestResponse>>(medicationRequest);
        return Result<IEnumerable<MedicationRequestResponse>>.Success(response);
    }
    
    public async Task<Result<MedicationRequestResponse>> CreateMedicationRequestAsync(CreateMedicationRequestRequest request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<MedicationRequestResponse>.Failure(result.Errors.First().ErrorMessage);
        }
        var medicationRequest = new MedicationRequest(
            Guid.NewGuid(),
            request.Quantity,
            request.WarehouseRequestId,
            request.MedicationId
        );

        try
        {
            await _repository.AddAsync(medicationRequest);
        }
        catch (Exception ex)
        {
            return Result<MedicationRequestResponse>.Failure($"Error al guardar la solicitud: {ex.Message}");
        }
        var response = _mapper.Map<MedicationRequestResponse>(medicationRequest);
        return Result<MedicationRequestResponse>.Success(response);
    }
    
    public async Task<Result<bool>> UpdateMedicationRequestAsync(Guid id, UpdateMedicationRequestRequest request)
    {
        var result = await _updateValidator.ValidateAsync(request);
        if(!result.IsValid)
        {
            return Result<bool>.Failure(result.Errors.First().ErrorMessage);
        }
        var medicationRequest = await _repository.GetByIdAsync(id);
        if(medicationRequest == null)
        {
            return Result<bool>.Failure("Solicitud de medicamentos al almacén no encontrada.");
        }

        try
        {
            await _repository.UpdateAsync(medicationRequest);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar la solicitud: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> DeleteMedicationRequestAsync(Guid id)
    {
        var medicationRequest = await _repository.GetByIdAsync(id);
        if(id == null)
        {
            return Result<bool>.Failure("Solicitud de medicamentos al almacén no encontrada.");
        }

        try
        {
            await _repository.DeleteAsync(medicationRequest);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la solicitud: {ex.Message}");
        }
    }
}