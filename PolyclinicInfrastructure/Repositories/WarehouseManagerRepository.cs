using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class WarehouseManagerRepository : Repository<WarehouseManager>, IWarehouseManagerRepository
{
    public WarehouseManagerRepository(AppDbContext context) : base(context)
    {
    }
}