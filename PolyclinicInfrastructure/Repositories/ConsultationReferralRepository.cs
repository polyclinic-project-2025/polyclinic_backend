using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace PolyclinicInfrastructure.Repositories;

public class ConsultationReferralRepository: Repository<ConsultationReferral>, IConsultationReferralRepository
{
    private readonly DbSet<ConsultationReferral> _dbSet;
    public ConsultationReferralRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<ConsultationReferral>();
    }
}