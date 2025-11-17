using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class NurseRepository : Repository<Nurse>, INurseRepository
{
    public NurseRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Nurse>> GetByNursingIdAsync(Guid nursingId)
        => await _context.Set<Nurse>()
            .Where(m => m.NursingId == nursingId)
            .ToListAsync();

}
