using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;


namespace PolyclinicDomain.IRepositories
{
    public interface IStockDepartmentRepository : IRepository<StockDepartment>
    {
        // Obtener todos los medicamentos y sus cantidades para un departamento
        Task<IEnumerable<StockDepartment>> GetStockByDepartmentAsync(Guid departmentId);

        // Obtener medicamentos cuyo stock está por debajo del mínimo
        Task<IEnumerable<StockDepartment>> GetBelowMinQuantityAsync(Guid departmentId);

        // Obtener medicamentos cuyo stock está por encima del máximo
        Task<IEnumerable<StockDepartment>> GetAboveMaxQuantityAsync(Guid departmentId);

        // Obtener stock específico por departamento y medicación
        Task<StockDepartment?> GetByDepartmentAndMedicationAsync(Guid departmentId, Guid medicationId);
    }
}
