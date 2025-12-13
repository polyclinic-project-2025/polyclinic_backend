using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class WarehouseRequestRepository :
    Repository<WarehouseRequest>,
    IWarehouseRequestRepository
{
    private readonly DbSet<WarehouseRequest> _dbSet;

    public WarehouseRequestRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<WarehouseRequest>();
    }

    public async Task<IEnumerable<WarehouseRequest>> GetByStatusAsync(string status)
        => await _dbSet.Where(wr => wr.Status == status)    
                        .ToListAsync();
    
    public async Task<IEnumerable<WarehouseRequest>> GetByDepartmentIdAsync(Guid departmentId)
        => await _dbSet.Where(wr => wr.DepartmentId == departmentId)
                        .ToListAsync();

    public async Task<IEnumerable<WarehouseRequest>> GetByStatusAndDepartmentIdAsync(string status, Guid departmentId)
        => await _dbSet.Where(wr => wr.Status == status && wr.DepartmentId == departmentId)
                        .ToListAsync();
}