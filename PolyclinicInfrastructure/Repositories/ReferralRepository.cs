using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicInfrastructure.Repositories
{
    public class ReferralRepository : Repository<Referral>, IReferralRepository{
        new private readonly AppDbContext _context;
    private readonly DbSet<Referral> _dbSet;

    public ReferralRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<Referral>();
    }

    public async Task<IEnumerable<Referral>> GetByPuestoExternoAsync(string PuestoExterno)
    {
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .Where(r => r.ExternalMedicalPost != null &&
                        r.ExternalMedicalPost.Name.ToLower().Contains(PuestoExterno.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Referral>> GetByDepartmentToNameAsync(string departmentName)
    {
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .Where(r => r.DepartmentTo != null &&
                        r.DepartmentTo.Name.ToLower().Contains(departmentName.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Referral>> GetByDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .Where(r => r.DateTimeRem.Date == date.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Referral>> GetByPatientNameAsync(string patientName)
    {
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .Where(r => r.Patient != null &&
                        r.Patient.Name.ToLower().Contains(patientName.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Referral>> GetByPatientIdentificationAsync(string patientIdentification)
    {
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .Where(r => r.Patient != null &&
                        r.Patient.Identification.ToLower().Contains(patientIdentification.ToLower()))
            .ToListAsync();
    }
    public async Task<Referral> GetByIdWithIncludesAsync(Guid id){
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .FirstOrDefaultAsync(r => r.ReferralId == id);
    }
    public async Task<IEnumerable<Referral>> GetAllWithIncludesAsync(){
        return await _dbSet
            .Include(r => r.ExternalMedicalPost)
            .Include(r => r.DepartmentTo)
            .Include(r => r.Patient)
            .ToListAsync();
    }
    }

}