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

    public async Task<ConsultationReferral?> GetByIdWithDeepIncludesAsync(Guid id)
    {
        return await _dbSet
            // 1. Incluir el Doctor
            .Include(c => c.Doctor)
                // 1a. Luego incluir el Departamento del Doctor
                .ThenInclude(d => d!.Department) 
            
            // 2. Incluir el Referral (Remisión)
            .Include(c => c.Referral)
                // 2a. Luego incluir el Paciente del Referral
                .ThenInclude(r => r!.Patient)
            
            // 3. Obtener el registro por ID
            .FirstOrDefaultAsync(c => c.ConsultationReferralId == id);
    }

    public async Task<IEnumerable<ConsultationReferral>> GetByDateRangeAsync(
        Guid patientId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _dbSet
            .Include(cr => cr.Referral)
                .ThenInclude(r => r.Patient)
            .Include(cr => cr.Referral)
                .ThenInclude(r => r.DepartmentTo)
            .Include(cr => cr.Doctor)
            .Where(cr =>
                cr.Referral!.PatientId == patientId &&
                cr.DateTimeCRem >= startDate &&
                cr.DateTimeCRem <= endDate)
            .OrderByDescending(cr => cr.DateTimeCRem)
            .ToListAsync();        
    }

    public async Task<IEnumerable<ConsultationReferral>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .Include(cr => cr.Referral)
                .ThenInclude(r => r.Patient)
            .Include(cr => cr.Referral)
                .ThenInclude(r => r.DepartmentTo)
            .Include(cr => cr.Doctor)
            .Where(cr => cr.Referral!.PatientId == patientId)
            .OrderByDescending(cr => cr.DateTimeCRem)
            .Take(10)
            .ToListAsync();
    }                
}