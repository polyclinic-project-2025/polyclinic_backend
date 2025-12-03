using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;
public interface IMedicationService
{
    // CRUD
    Task<Result<MedicationDto>> CreateAsync(CreateMedicationDto request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<MedicationDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<MedicationDto>>> GetAllAsync();

    // Búsquedas
    Task<Result<MedicationDto>> GetByBatchNumberAsync(string batchNumber);
    Task<Result<IEnumerable<MedicationDto>>> GetByCommercialCompanyAsync(string company);
    Task<Result<IEnumerable<MedicationDto>>> SearchByNameAsync(string name);

    // Métodos especiales de stock
    Task<Result<IEnumerable<MedicationDto>>> GetLowStockWarehouseAsync();
    Task<Result<IEnumerable<MedicationDto>>> GetLowStockNurseAsync();
    Task<Result<IEnumerable<MedicationDto>>> GetOverStockWarehouseAsync();
    Task<Result<IEnumerable<MedicationDto>>> GetOverStockNurseAsync();
}