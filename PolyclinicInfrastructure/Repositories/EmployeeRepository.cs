using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public abstract class EmployeeRepository<TEntity> : 
    Repository<TEntity>, 
    IEmployeeRepository<TEntity>
    where TEntity : Employee
{
    private readonly DbSet<TEntity> _dbSet;

    public EmployeeRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdentificationAsync(string identification)
        => await _dbSet.FirstOrDefaultAsync(d => d.Identification == identification);

    public async Task<bool> ExistsByIdentificationAsync(string identification)
        => await _dbSet.AnyAsync(d => d.Identification == identification);
}