using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class PuestoExternoRepository : Repository<ExternalMedicalPost>, IPuestoExternoRepository
{
    new private readonly AppDbContext _context;
    private readonly DbSet<ExternalMedicalPost> _dbSet;

    public PuestoExternoRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<ExternalMedicalPost>();
    }

    public async Task<ExternalMedicalPost?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Name == name);
    }
}