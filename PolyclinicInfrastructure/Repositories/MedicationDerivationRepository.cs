using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class MedicationDerivationRepository : Repository<MedicationDerivation>, IMedicationDerivationRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<MedicationDerivation> _dbSet;

    public MedicationDerivationRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<MedicationDerivation>();
    }

    public async Task<IEnumerable<MedicationDerivation>> GetByConsultationDerivationIdAsync(Guid consultationDerivationId)
    {
        return await _dbSet
            .Include(md => md.Medication) 
            .Where(md => md.ConsultationDerivationId == consultationDerivationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MedicationDerivation>> GetByMedicationIdAsync(Guid medicationId)
    {
        return await _dbSet
            .Include(md => md.Medication)
            .Where(md => md.MedicationId == medicationId)
            .ToListAsync();
    }
}