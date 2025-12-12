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
    public class EmergencyRoomRepository : Repository<EmergencyRoom>, IEmergencyRoomRepository
    {
        private readonly DbSet<EmergencyRoom> _dbSet;
        
        public EmergencyRoomRepository(AppDbContext context) : base(context)
        {
            _dbSet = _context.Set<EmergencyRoom>();
        }
        // Métodos específicos con includes
        public async Task<IEnumerable<EmergencyRoom>> GetByDateAsync(DateOnly date)
        {
            return await _dbSet
                .Include(er => er.Doctor) // Incluye Doctor para el DTO
                .Where(er => er.GuardDate == date)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoom>> GetByDoctorIdentificationAsync(string doctorIdentification)
        {
            return await _dbSet
                .Include(er => er.Doctor) // Incluye Doctor para el DTO y el filtro
                .Where(er => er.Doctor.Identification == doctorIdentification)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoom>> GetByDoctorNameAsync(string doctorName)
        {
            return await _dbSet
                .Include(er => er.Doctor) // Incluye Doctor para el DTO y el filtro
                .Where(er => er.Doctor.Name.Contains(doctorName))
                .ToListAsync();
        }
        public async Task<EmergencyRoom?> GetByIdWithDoctorAsync(Guid id)
        {
            return await _dbSet
                .Include(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .FirstOrDefaultAsync(er => er.EmergencyRoomId == id);
        }

        public async Task<IEnumerable<EmergencyRoom>> GetAllWithDoctorAsync()
        {
            return await _dbSet
                .Include(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .ToListAsync();
        }
        public async Task<bool> IsDoctorOnGuardAsync(Guid doctorId, DateOnly date)
        {
            return await _dbSet
                .AnyAsync(er => er.DoctorId == doctorId && er.GuardDate == date);
        }
    }
}