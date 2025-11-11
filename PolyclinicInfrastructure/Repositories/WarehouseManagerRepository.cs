using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class WarehouseManagerRepository : Repository<WarehouseManager>, IWarehouseManagerRepository
{
    public WarehouseManagerRepository(AppDbContext context) : base(context) { }

    public async Task<WarehouseManager?> GetByManagedWarehouseAsync(Guid managedWarehouseId)
        => await _context.Set<WarehouseManager>()
            .FirstOrDefaultAsync(w => w.ManagedWarehouseId == managedWarehouseId);
}
