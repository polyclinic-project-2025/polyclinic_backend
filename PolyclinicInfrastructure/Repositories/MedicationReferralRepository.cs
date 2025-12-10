using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class MedicationReferralRepository : Repository<MedicationReferral>, IMedicationReferralRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<MedicationReferral> _dbSet;

    public MedicationReferralRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<MedicationReferral>();
    }

    public async Task<IEnumerable<MedicationReferral>> GetByConsultationReferralIdAsync(Guid consultationReferralId)
    {
        return await _dbSet
            .Include(mr => mr.Medication)
            .Where(mr => mr.ConsultationReferralId == consultationReferralId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicationReferral>> GetByMedicationIdAsync(Guid medicationId)
    {
        return await _dbSet
            .Include(mr => mr.Medication)
            .Where(mr => mr.MedicationId == medicationId)
            .ToListAsync();
    }
}
