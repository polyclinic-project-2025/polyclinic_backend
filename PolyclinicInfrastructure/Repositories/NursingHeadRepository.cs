using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class NursingHeadRepository : Repository<NursingHead>, INursingHeadRepository
{
    public NursingHeadRepository(AppDbContext context) : base(context)
    {
    }
}
