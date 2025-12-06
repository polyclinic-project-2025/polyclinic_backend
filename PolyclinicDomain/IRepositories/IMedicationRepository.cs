using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IMedicationRepository : IRepository<Medication>
{
    // Buscar por número de lote (campo crítico)
        Task<Medication?> GetByBatchNumberAsync(string batchNumber);

        // Verificar existencia del lote antes de crear o actualizar
        Task<bool> ExistsBatchAsync(string batchNumber);

        // Verificar existencia de medicamento con ese nombre 
        Task<bool> ExistsMedicationAsync(string name);

        // Buscar por compañía comercial
        Task<IEnumerable<Medication>> GetByCommercialCompanyAsync(string company);

        // Buscar por coincidencia parcial en nombre comercial o científico
        Task<IEnumerable<Medication>> SearchByNameAsync(string name);

        // Medicamentos con stock bajo en Almacén (comparado con MinQuantityWarehouse)
        Task<IEnumerable<Medication>> GetLowStockWarehouseAsync();

        // Medicamentos con stock bajo en Enfermería (comparado con MinQuantityNurse)
        Task<IEnumerable<Medication>> GetLowStockNurseAsync();

        // Medicamentos que exceden cantidades máximas en Almacén
        Task<IEnumerable<Medication>> GetOverstockWarehouseAsync();

        // Medicamentos que exceden cantidades máximas en Enfermería
        Task<IEnumerable<Medication>> GetOverstockNurseAsync();
}