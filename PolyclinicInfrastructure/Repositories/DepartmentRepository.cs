using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    new private readonly AppDbContext _context;
    private readonly DbSet<Department> _dbSet;

    public DepartmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<Department>();
    }

    public async Task<Department?> GetWithHeadAsync(Guid id)
    {
        // Eager load de relaciones importantes en una sola query
        return await _dbSet
            .Include(d => d.DepartmentHead)
            .Include(d => d.StockDepartments)
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<Department?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Name == name);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(d => d.Name == name);
    }
}
