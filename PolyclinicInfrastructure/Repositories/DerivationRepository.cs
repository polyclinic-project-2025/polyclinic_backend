using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PolyclinicInfrastructure.Repositories{
    public class DerivationRepository : Repository<Derivation>, IDerivationRepository
{
    new private readonly AppDbContext _context;
    private readonly DbSet<Derivation> _dbSet;

    public DerivationRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<Derivation>();
    }

    // Obtener derivaciones por nombre de departamento origen
    public async Task<IEnumerable<Derivation>> GetByDepartmentFromNameAsync(string departmentName)
    {
        return await _dbSet
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Include(d => d.Patient)
            .Where(d => d.DepartmentFrom != null &&
                        d.DepartmentFrom.Name.ToLower().Contains(departmentName.ToLower()))
            .ToListAsync();
    }

    // Obtener derivaciones por nombre de departamento destino
    public async Task<IEnumerable<Derivation>> GetByDepartmentToNameAsync(string departmentName)
    {
        return await _dbSet
            .Include(d => d.DepartmentTo)
            .Include(d => d.Patient)
            .Include(d => d.DepartmentFrom)
            .Where(d => d.DepartmentTo != null &&
                        d.DepartmentTo.Name.ToLower().Contains(departmentName.ToLower()))
            .ToListAsync();
    }

    // Obtener derivaciones por fecha exacta
    public async Task<IEnumerable<Derivation>> GetByDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Include(d => d.Patient)
            .Where(d => d.DateTimeDer.Date == date.Date)
            .ToListAsync();
    }

    // Obtener derivaciones por nombre del paciente
    public async Task<IEnumerable<Derivation>> GetByPatientNameAsync(string patientName)
    {
        return await _dbSet
            .Include(d => d.Patient)
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Where(d => d.Patient != null &&
                        d.Patient.Name.ToLower().Contains(patientName.ToLower()))
            .ToListAsync();
    }

    //Obtener derivaciones por identificacion del paciente
    public async Task<IEnumerable<Derivation>> GetByPatientIdentificationAsync(string patientIdentification)
    {
        return await _dbSet
            .Include(d => d.Patient)
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Where(d => d.Patient != null &&
                        d.Patient.Identification.ToLower().Contains(patientIdentification.ToLower()))
            .ToListAsync();
    }
    public async Task<Derivation> GetByIdWithIncludesAsync(Guid id){
        return await _dbSet
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Include(d => d.Patient)
            .FirstOrDefaultAsync(d => d.DerivationId == id);
    }
    public async Task<IEnumerable<Derivation>> GetAllWithIncludesAsync(){
        return await _dbSet
            .Include(d => d.DepartmentFrom)
            .Include(d => d.DepartmentTo)
            .Include(d => d.Patient)
            .ToListAsync();
    }
} 
}

