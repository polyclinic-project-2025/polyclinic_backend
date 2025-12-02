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
    private readonly IValidator<CreateMedicationRequest> _createValidator;
    private readonly IValidator<UpdateMedicationRequest> _updateValidator;

    public MedicationService(
        IMedicationRepository repository,
        IMapper mapper,
        IValidator<CreateMedicationRequest> createValidator,
        IValidator<UpdateMedicationRequest> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // ============================================================
    // CRUD
    // ============================================================

    public async Task<Result<MedicationResponse>> CreateAsync(CreateMedicationRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<MedicationResponse>.Failure(validation.Errors.First().ErrorMessage);

        // BatchNumber debe ser único
        if (await _repository.ExistsBatchAsync(request.BatchNumber))
            return Result<MedicationResponse>.Failure("Ya existe un medicamento con este número de lote.");

        var medication = _mapper.Map<Medication>(request);
        medication = await _repository.AddAsync(medication);

        var response = _mapper.Map<MedicationResponse>(medication);
        return Result<MedicationResponse>.Success(response);
    }

    public async Task<Result<MedicationResponse>> GetByIdAsync(Guid id)
    {
        var medication = await _repository.GetByIdAsync(id);
        if (medication == null)
            return Result<MedicationResponse>.Failure("Medicamento no encontrado.");

        var response = _mapper.Map<MedicationResponse>(medication);
        
        return Result<MedicationResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> GetAllAsync()
    {
        var medications = await _repository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var medication = await _repository.GetByIdAsync(id);
        if (medication == null)
            return Result<bool>.Failure("Medicamento no encontrado.");

        // BatchNumber no se debe modificar así que no lo mapeamos
        _mapper.Map(request, medication);

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

    public async Task<Result<MedicationResponse>> GetByBatchNumberAsync(string batchNumber)
    {
        var medication = await _repository.GetByBatchNumberAsync(batchNumber);
        if (medication == null)
            return Result<MedicationResponse>.Failure("No existe un medicamento con ese número de lote.");
        
        var response = _mapper.Map<MedicationResponse>(medication);
        
        return Result<MedicationResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> GetByCommercialCompanyAsync(string company)
    {
        var medications = await _repository.GetByCommercialCompanyAsync(company);
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> SearchByNameAsync(string name)
    {
        var medications = await _repository.SearchByNameAsync(name);
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    // ============================================================
    // MÉTODOS ESPECIALES DE STOCK
    // ============================================================

    public async Task<Result<IEnumerable<MedicationResponse>>> GetLowStockWarehouseAsync()
    {
        var medications = await _repository.GetLowStockWarehouseAsync();
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> GetLowStockNurseAsync()
    {
        var medications = await _repository.GetLowStockNurseAsync();
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> GetOverStockWarehouseAsync()
    {
        var medications = await _repository.GetOverstockWarehouseAsync();
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationResponse>>> GetOverStockNurseAsync()
    {
        var medications = await _repository.GetOverstockNurseAsync();
        var response = _mapper.Map<IEnumerable<MedicationResponse>>(medications);
        return Result<IEnumerable<MedicationResponse>>.Success(response);
    }
}