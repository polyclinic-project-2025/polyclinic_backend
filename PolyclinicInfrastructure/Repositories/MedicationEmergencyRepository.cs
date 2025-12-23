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
    public class MedicationEmergencyRepository : Repository<MedicationEmergency>, IMedicationEmergencyRepository
    {
        private readonly DbSet<MedicationEmergency> _dbSet;
        
        public MedicationEmergencyRepository(AppDbContext context) : base(context)
        {
            _dbSet = _context.Set<MedicationEmergency>();
        }

        // Métodos específicos con includes
        public async Task<IEnumerable<MedicationEmergency>> GetByEmergencyRoomCareIdAsync(Guid emergencyRoomCareId)
        {
            return await _dbSet
                .Include(me => me.Medication) // Para MedicationName
                .Where(me => me.EmergencyRoomCareId == emergencyRoomCareId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationEmergency>> GetByMedicationIdAsync(Guid medicationId)
        {
            return await _dbSet
                .Include(me => me.Medication) // Para MedicationName
                .Where(me => me.MedicationId == medicationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationEmergency>> GetByMedicationNameAsync(string medicationName)
        {
            return await _dbSet
                .Include(me => me.Medication) // Para poder filtrar por Medication.Name
                .Where(me => me.Medication.CommercialName.Contains(medicationName))
                .ToListAsync();
        }
        public async Task<MedicationEmergency?> GetByIdWithMedicationAsync(Guid id)
        {
            return await _dbSet
                .Include(me => me.Medication) // Para MedicationName
                .FirstOrDefaultAsync(me => me.MedicationEmergencyId == id);
        }

        public async Task<IEnumerable<MedicationEmergency>> GetAllWithMedicationAsync()
        {
            return await _dbSet
                .Include(me => me.Medication) // Para MedicationName
                .ToListAsync();
        }
    }
}