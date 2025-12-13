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

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _repository;
    private readonly IMapper _mapper;

    public MedicationService(
        IMedicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // ============================================================
    // CRUD
    // ============================================================

    public async Task<Result<MedicationDto>> CreateAsync(CreateMedicationDto request)
    {
        try
        {
            // BatchNumber debe ser único
            if (await _repository.ExistsBatchAsync(request.BatchNumber) && await _repository.ExistsMedicationAsync(request.CommercialName) && await _repository.ExistsMedicationAsync(request.ScientificName))
                return Result<MedicationDto>.Failure("Ya existe un medicamento con este número de lote.");

            var expirationDate = DateOnly.ParseExact(request.ExpirationDate,"yyyy-MM-dd");    

            var medication = new Medication(
                Guid.NewGuid(),                    
                request.Format,                    
                request.CommercialName,            
                request.CommercialCompany,         
                request.BatchNumber,               
                request.ScientificName,            
                expirationDate,                    
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
        catch (Exception ex)
        {
            return Result<MedicationDto>.Failure($"Error al guardar el medicamento: {ex.Message}");
        }
    }

    public async Task<Result<MedicationDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var medication = await _repository.GetByIdAsync(id);
            if (medication == null)
                return Result<MedicationDto>.Failure("Medicamento no encontrado.");

            var response = _mapper.Map<MedicationDto>(medication);
            
            return Result<MedicationDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationDto>.Failure($"Error al obtener el medicamento: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetAllAsync()
    {
        try
        {
            var medications = await _repository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al obtener medicamentos: {ex.Message}");
        }
    }

     public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDto request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);
        try
        {
            var medication = await _repository.GetByIdAsync(id);
            if (medication == null)
                return Result<bool>.Failure("Medicamento no encontrado.");

            medication.UpdateFormat(request.Format);
            medication.UpdateCommercialName(request.CommercialName);
            medication.UpdateCommercialCompany(request.CommercialCompany);
            
            // Convertir string a DateOnly
            if (DateOnly.TryParse(request.ExpirationDate, out var expirationDate))
            {
                medication.UpdateExpirationDate(expirationDate);
            }
            
            medication.UpdateScientificName(request.ScientificName);
            medication.UpdateQuantityWarehouse(request.QuantityWarehouse);
            medication.UpdateQuantityNurse(request.QuantityNurse);

            await _repository.UpdateAsync(medication);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            medication.UpdateScientificName(request.ScientificName);
            return Result<bool>.Failure($"Error al actualizar el medicamento: {ex.Message}");
        }

        await _repository.UpdateAsync(medication);
        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var medication = await _repository.GetByIdAsync(id);
            if (medication == null)
                return Result<bool>.Failure("Medicamento no encontrado.");

            await _repository.DeleteAsync(medication);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar el medicamento: {ex.Message}");
        }
    }

    // ============================================================
    // BÚSQUEDAS ESPECIALES
    // ============================================================

    public async Task<Result<MedicationDto>> GetByBatchNumberAsync(string batchNumber)
    {
        try
        {
            var medication = await _repository.GetByBatchNumberAsync(batchNumber);
            if (medication == null)
                return Result<MedicationDto>.Failure("No existe un medicamento con ese número de lote.");
            
            var response = _mapper.Map<MedicationDto>(medication);
            
            return Result<MedicationDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationDto>.Failure($"Error al buscar medicamento por lote: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetByCommercialCompanyAsync(string company)
    {
        try
        {
            var medications = await _repository.GetByCommercialCompanyAsync(company);
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al buscar medicamentos por compañía: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> SearchByNameAsync(string name)
    {
        try
        {
            var medications = await _repository.SearchByNameAsync(name);
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al buscar medicamentos por nombre: {ex.Message}");
        }
    }

    // ============================================================
    // MÉTODOS ESPECIALES DE STOCK
    // ============================================================

    public async Task<Result<IEnumerable<MedicationDto>>> GetLowStockWarehouseAsync()
    {
        try
        {
            var medications = await _repository.GetLowStockWarehouseAsync();
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al obtener medicamentos con stock bajo en almacén: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetLowStockNurseAsync()
    {
        try
        {
            var medications = await _repository.GetLowStockNurseAsync();
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al obtener medicamentos con stock bajo en enfermería: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetOverStockWarehouseAsync()
    {
        try
        {
            var medications = await _repository.GetOverstockWarehouseAsync();
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al obtener medicamentos con sobrestock en almacén: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDto>>> GetOverStockNurseAsync()
    {
        try
        {
            var medications = await _repository.GetOverstockNurseAsync();
            var response = _mapper.Map<IEnumerable<MedicationDto>>(medications);
            return Result<IEnumerable<MedicationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDto>>.Failure($"Error al obtener medicamentos con sobrestock en enfermería: {ex.Message}");
        }
    }
}