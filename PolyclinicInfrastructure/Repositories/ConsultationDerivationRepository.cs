using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class ConsultationDerivationRepository 
    : Repository<ConsultationDerivation>, IConsultationDerivationRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<ConsultationDerivation> _dbSet;

    public ConsultationDerivationRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
        _dbSet = _context.Set<ConsultationDerivation>();
    }

    // -----------------------------------------------------------
    // Custom Methods
    // -----------------------------------------------------------

    public async Task<IEnumerable<ConsultationDerivation>> GetByDateRangeAsync(
        Guid patientId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _dbSet
            .Include(cd => cd.Derivation)
                .ThenInclude(d => d.Patient)
            .Include(cd => cd.Derivation)
                .ThenInclude(d => d.DepartmentTo)
            .Include(cd => cd.Doctor)
            .Include(cd => cd.DepartmentHead)
            .Where(cd =>
                cd.Derivation!.PatientId == patientId &&
                cd.DateTimeCDer >= startDate &&
                cd.DateTimeCDer <= endDate)
            .OrderByDescending(cd => cd.DateTimeCDer)
            .ToListAsync();
    }

    public async Task<IEnumerable<ConsultationDerivation>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .Include(cd => cd.Derivation)
                .ThenInclude(d => d.Patient)
            .Include(cd => cd.Derivation)
                .ThenInclude(d => d.DepartmentTo)
            .Include(cd => cd.Doctor)
            .Include(cd => cd.DepartmentHead)
            .Where(cd => cd.Derivation!.PatientId == patientId)
            .OrderByDescending(cd => cd.DateTimeCDer)
            .Take(10)
            .ToListAsync();
    }
}
