using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class DepartmentHeadRepository : Repository<DepartmentHead>, IDepartmentHeadRepository
{
    public DepartmentHeadRepository(AppDbContext context) : base(context) { }

    public async Task<DepartmentHead?> GetByDepartmentAsync(Guid managedDepartmentId)
        => await _context.Set<DepartmentHead>()
            .FirstOrDefaultAsync(d => d.ManagedDepartmentId == managedDepartmentId);
}