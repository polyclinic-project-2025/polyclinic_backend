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
    Task<MedicationResponse> CreateAsync(CreateMedicationRequest request);
    Task<MedicationResponse> UpdateAsync(Guid id, UpdateMedicationRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<MedicationResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicationResponse>> GetAllAsync();

    // Búsquedas
    Task<MedicationResponse?> GetByBatchNumberAsync(string batchNumber);
    Task<IEnumerable<MedicationResponse>> GetByCommercialCompanyAsync(string company);
    Task<IEnumerable<MedicationResponse>> SearchByNameAsync(string name);

    // Métodos especiales de stock
    Task<IEnumerable<MedicationResponse>> GetLowStockWarehouseAsync();
    Task<IEnumerable<MedicationResponse>> GetLowStockNurseAsync();
    Task<IEnumerable<MedicationResponse>> GetOverStockWarehouseAsync();
    Task<IEnumerable<MedicationResponse>> GetOverStockNurseAsync();
}