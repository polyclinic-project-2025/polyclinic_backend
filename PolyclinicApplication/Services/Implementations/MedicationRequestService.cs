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
public class MedicationRequestService : IMedicationRequestService
{
    private readonly IMedicationRequestRepository _repository;
    private readonly IMapper _mapper;

    public MedicationRequestService(
        IMedicationRequestRepository repository,
        IMapper mapper
    )
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<MedicationRequestResponse>>> GetAllMedicationRequestAsync()
    {
        try
        {
            var medicationRequest = await _repository.GetAllAsync();
            var responses = _mapper.Map<IEnumerable<MedicationRequestResponse>>(medicationRequest);
            return Result<IEnumerable<MedicationRequestResponse>>.Success(responses);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationRequestResponse>>.Failure($"Error al obtener solicitudes: {ex.Message}");
        }
    }

    public async Task<Result<MedicationRequestResponse>> GetMedicationRequestByIdAsync(Guid id)
    {
        try
        {
            var medicationRequest = await _repository.GetByIdAsync(id);
            if(medicationRequest == null)
            {
                return Result<MedicationRequestResponse>.Failure("Solicitud de medicamentos al almacén no encontrada.");
            }
            var response = _mapper.Map<MedicationRequestResponse>(medicationRequest);
            return Result<MedicationRequestResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationRequestResponse>.Failure($"Error al obtener solicitud: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationRequestResponse>>> GetMedicationRequestByWarehouseRequestIdAsync(Guid warehouseRequestId)
    {
        try
        {
            var medicationRequest = await _repository.GetByWarehouseRequestIdAsync(warehouseRequestId);
            if(medicationRequest == null)
            {
                return Result<IEnumerable<MedicationRequestResponse>>.Failure("Solicitud al almacén no encontrada.");
            }
            var response = _mapper.Map<IEnumerable<MedicationRequestResponse>>(medicationRequest);
            return Result<IEnumerable<MedicationRequestResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationRequestResponse>>.Failure($"Error al obtener solicitud: {ex.Message}");
        }
    }
    
    public async Task<Result<MedicationRequestResponse>> CreateMedicationRequestAsync(CreateMedicationRequestRequest request)
    {
        try
        {
            var medicationRequest = new MedicationRequest(
                Guid.NewGuid(),
                request.Quantity,
                request.WarehouseRequestId,
                request.MedicationId
            );

            await _repository.AddAsync(medicationRequest);
            
            var response = _mapper.Map<MedicationRequestResponse>(medicationRequest);
            return Result<MedicationRequestResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationRequestResponse>.Failure($"Error al guardar la solicitud: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> UpdateMedicationRequestAsync(Guid id, UpdateMedicationRequestRequest request)
    {
        try
        {
            var medicationRequest = await _repository.GetByIdAsync(id);
            if(medicationRequest == null)
            {
                return Result<bool>.Failure("Solicitud de medicamentos al almacén no encontrada.");
            }

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
        try
        {
            var medicationRequest = await _repository.GetByIdAsync(id);
            if(id == null)
            {
                return Result<bool>.Failure("Solicitud de medicamentos al almacén no encontrada.");
            }

            await _repository.DeleteAsync(medicationRequest);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la solicitud: {ex.Message}");
        }
    }
}