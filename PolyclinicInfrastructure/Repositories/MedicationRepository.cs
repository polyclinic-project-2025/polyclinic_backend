using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class MedicationRepository : Repository<Medication>, IMedicationRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Medication> _dbSet;

    public MedicationRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<Medication>();
    }

    // Buscar por número de lote (único en inventarios)
    public async Task<Medication?> GetByBatchNumberAsync(string batchNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.BatchNumber == batchNumber);
    }

    // Verificar si el lote existe
    public async Task<bool> ExistsBatchAsync(string batchNumber)
    {
        return await _dbSet
            .AnyAsync(m => m.BatchNumber == batchNumber);
    }

    //Verificar si ese nombre ya existe
    public async Task<bool> ExistsMedicationAsync(string name)
    {
        return await _dbSet
            .AnyAsync(m => m.CommercialName == name || m.ScientificName == name);
    }
    // Buscar medicamentos por empresa comercial
    public async Task<IEnumerable<Medication>> GetByCommercialCompanyAsync(string company)
    {
        return await _dbSet
            .Where(m => m.CommercialCompany == company)
            .ToListAsync();
    }

    // Búsqueda por coincidencia parcial (nombre comercial o científico)
    public async Task<IEnumerable<Medication>> SearchByNameAsync(string name)
    {
        name = name.Trim();

        return await _dbSet
            .Where(m =>
                EF.Functions.ILike(m.CommercialName, $"%{name}%") ||
                EF.Functions.ILike(m.ScientificName, $"%{name}%"))
            .ToListAsync();
    }       
    
    // Obtener medicamentos con stock bajo
    // STOCK BAJO - ALMACÉN
    public async Task<IEnumerable<Medication>> GetLowStockWarehouseAsync()
    {
        return await _dbSet
            .Where(m => m.QuantityWarehouse < m.MinQuantityWarehouse)
            .ToListAsync();
    }

    // STOCK BAJO - ENFERMERÍA
    public async Task<IEnumerable<Medication>> GetLowStockNurseAsync()
    {
        return await _dbSet
            .Where(m => m.QuantityNurse < m.MinQuantityNurse)
            .ToListAsync();
    }

    // STOCK ALTO - ALMACÉN
    public async Task<IEnumerable<Medication>> GetOverstockWarehouseAsync()
    {
        return await _dbSet
            .Where(m => m.QuantityWarehouse > m.MaxQuantityWarehouse)
            .ToListAsync();
    }

    // STOCK ALTO - ENFERMERÍA
    public async Task<IEnumerable<Medication>> GetOverstockNurseAsync()
    {
        return await _dbSet
            .Where(m => m.QuantityNurse > m.MaxQuantityNurse)
            .ToListAsync();
    }
}