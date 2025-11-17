using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class NursingHeadRepository : Repository<NursingHead>, INursingHeadRepository
{
    public NursingHeadRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<NursingHead>> GetByManagedNursingId(Guid managedNursingId)
        => await _context.Set<NursingHead>()
            .Where(h => h.ManagedNursingId == managedNursingId)
            .ToListAsync();
}
