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

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMedicationDto> _createValidator;
    private readonly IValidator<UpdateMedicationDto> _updateValidator;

    public MedicationService(
        IMedicationRepository repository,
        IMapper mapper,
        IValidator<CreateMedicationDto> createValidator,
        IValidator<UpdateMedicationDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // ============================================================
    // CRUD
    // ============================================================

    public async Task<Result<MedicationDto>> CreateAsync(CreateMedicationDto request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<MedicationDto>.Failure(validation.Errors.First().ErrorMessage);

        // BatchNumber debe ser único
        if (await _repository.ExistsBatchAsync(request.BatchNumber))
            return Result<MedicationDto>.Failure("Ya existe un medicamento con este número de lote.");

        var medication = new Medication(
            Guid.NewGuid(),
            request.CommercialName,
            request.ScientificName,
            request.Format,
            request.CommercialCompany,
            request.BatchNumber,
            request.QuantityWarehouse,
            request.QuantityNurse,
            request.MinQuantityWarehouse,
            request.MinQuantityNurse,
            request.MaxQuantityWarehouse,
            request.MaxQuantityNurse
        );
        medication = await _repository.AddAsync(medication);

        var response = _mapper.Map<MedicationDto>(medication);
        return Result<MedicationDto>.Success(response);
    }

    public async Task<Result<MedicationDto>> GetByIdAsync(Guid id)
    {
        var medication = await _repository.GetByIdAsync(id);
        if (medication == null)
            return Result<MedicationDto>.Failure("Medicamento no encontrado.");

        var response = _mapper.Map<MedicationDto>(medication);
        
        return Result<MedicationDto>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetAllAsync()
    {
        var medications = await _repository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDto request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var medication = await _repository.GetByIdAsync(id);
        if (medication == null)
            return Result<bool>.Failure("Medicamento no encontrado.");

        // Actualizar sólo campos provistos en el request
        if (!string.IsNullOrEmpty(request.Format))
        {
            medication.UpdateFormat(request.Format);
        }

        if(!string.IsNullOrEmpty(request.CommercialName))
        {
            medication.UpdateCommercialName(request.CommercialName);
        }
    
        // Si tienes otros campos editables (por ejemplo CommercialCompany), actualízalos aquí:
        if (!string.IsNullOrEmpty(request.CommercialCompany))
        {
            // Asume que existe un método de dominio UpdateCommercialCompany
            medication.UpdateCommercialCompany(request.CommercialCompany);
        }

        if(request.ExpirationDate != default)
        {
            medication.UpdateExpirationDate(request.ExpirationDate);
        }

        if(!string.IsNullOrEmpty(request.ScientificName))
        {
            medication.UpdateScientificName(request.ScientificName);
        }

        await _repository.UpdateAsync(medication);
        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var medication = await _repository.GetByIdAsync(id);
        if (medication == null)
            return Result<bool>.Failure("Medicamento no encontrado.");

        await _repository.DeleteAsync(medication);
        return Result<bool>.Success(true);
    }

    // ============================================================
    // BÚSQUEDAS ESPECIALES
    // ============================================================

    public async Task<Result<MedicationDto>> GetByBatchNumberAsync(string batchNumber)
    {
        var medication = await _repository.GetByBatchNumberAsync(batchNumber);
        if (medication == null)
            return Result<MedicationDto>.Failure("No existe un medicamento con ese número de lote.");
        
        var response = _mapper.Map<MedicationDto>(medication);
        
        return Result<MedicationDto>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetByCommercialCompanyAsync(string company)
    {
        var medications = await _repository.GetByCommercialCompanyAsync(company);
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> SearchByNameAsync(string name)
    {
        var medications = await _repository.SearchByNameAsync(name);
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    // ============================================================
    // MÉTODOS ESPECIALES DE STOCK
    // ============================================================

    public async Task<Result<IEnumerable<MedicationDto>>> GetLowStockWarehouseAsync()
    {
        var medications = await _repository.GetLowStockWarehouseAsync();
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetLowStockNurseAsync()
    {
        var medications = await _repository.GetLowStockNurseAsync();
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetOverStockWarehouseAsync()
    {
        var medications = await _repository.GetOverstockWarehouseAsync();
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetOverStockNurseAsync()
    {
        var medications = await _repository.GetOverstockNurseAsync();
        var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
        return Result<IEnumerable<MedicationDto>>.Success(response);
    }
}