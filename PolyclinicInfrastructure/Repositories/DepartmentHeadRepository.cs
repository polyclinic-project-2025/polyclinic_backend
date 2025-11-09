using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class DepartmentHeadRepository : Repository<DepartmentHead>, IDepartmentHeadRepository
{
    public DepartmentHeadRepository(AppDbContext context) : base(context)
    {
    }
}