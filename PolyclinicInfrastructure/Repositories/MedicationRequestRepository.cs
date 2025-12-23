using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class MedicationRequestRepository :
    Repository<MedicationRequest>,
    IMedicationRequestRepository
{
    private readonly DbSet<MedicationRequest> _dbSet;
    
    public MedicationRequestRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<MedicationRequest>();
    }

    public async Task<IEnumerable<MedicationRequest>> GetByWarehouseRequestIdAsync(Guid warehouseRequestId)
        => await _dbSet.Where(mr => mr.WarehouseRequestId == warehouseRequestId)
                        .ToListAsync();
}