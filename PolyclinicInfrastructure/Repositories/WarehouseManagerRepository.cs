using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class WarehouseManagerRepository :
    EmployeeRepository<WarehouseManager>, IWarehouseManagerRepository
{
    private readonly DbSet<WarehouseManager> _dbSet;

    public WarehouseManagerRepository(AppDbContext context) : base(context) 
    {
        _dbSet = _context.Set<WarehouseManager>();
    }

    public async Task<WarehouseManager?> GetAsync()
        => await _dbSet.OrderByDescending(wm => wm.AssignedAt)
                        .FirstOrDefaultAsync();
}