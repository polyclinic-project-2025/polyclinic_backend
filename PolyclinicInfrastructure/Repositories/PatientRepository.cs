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
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        new private readonly AppDbContext _context;
        private readonly DbSet<Patient> _dbSet;

        public PatientRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Patient>();
        }

        // Buscar pacientes por nombre (búsqueda parcial)
        public async Task<IEnumerable<Patient>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        // Buscar paciente por identificación (exacta)
        public async Task<Patient?> GetByIdentificationAsync(string identification)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Identification == identification);
        }

        // Buscar pacientes por edad
        public async Task<IEnumerable<Patient>> GetByAgeAsync(int age)
        {
            return await _dbSet
                .Where(p => p.Age == age)
                .ToListAsync();
        }

        // Traer paciente con todas sus relaciones cargadas
        public async Task<Patient?> GetPatientWithAllRelationsAsync(Guid patientId)
        {
            return await _dbSet
                .Include(p => p.Derivations)
                .Include(p => p.Referrals)
                .Include(p => p.ConsultationDerivations)
                .Include(p => p.ConsultationReferrals)
                .Include(p => p.EmergencyRoomCares)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }
    }
}
