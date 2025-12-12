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
    public class EmergencyRoomCareRepository : Repository<EmergencyRoomCare>, IEmergencyRoomCareRepository
    {
        private readonly DbSet<EmergencyRoomCare> _dbSet;
        
        public EmergencyRoomCareRepository(AppDbContext context) : base(context)
        {
            _dbSet = _context.Set<EmergencyRoomCare>();
        }

        // Métodos específicos con includes
        public async Task<IEnumerable<EmergencyRoomCare>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom) // Para Doctor
                    .ThenInclude(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .Where(erc => erc.CareDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoomCare>> GetByDoctorNameAsync(string doctorName)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom)
                    .ThenInclude(er => er.Doctor)
                .Where(erc => erc.EmergencyRoom.Doctor.Name.Contains(doctorName))
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoomCare>> GetByDoctorIdentificationAsync(string doctorIdentification)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom)
                    .ThenInclude(er => er.Doctor)
                .Where(erc => erc.EmergencyRoom.Doctor.Identification == doctorIdentification)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoomCare>> GetByPatientNameAsync(string patientName)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom) // Para Doctor
                    .ThenInclude(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .Where(erc => erc.Patient.Name.Contains(patientName))
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRoomCare>> GetByPatientIdentificationAsync(string patientIdentification)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom) // Para Doctor
                    .ThenInclude(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .Where(erc => erc.Patient.Identification == patientIdentification)
                .ToListAsync();
        }
        public async Task<EmergencyRoomCare?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(erc => erc.Patient) // Para PatientName y PatientIdentification
                .Include(erc => erc.EmergencyRoom) // Para llegar al Doctor
                    .ThenInclude(er => er.Doctor) // Para DoctorName y DoctorIdentification
                .FirstOrDefaultAsync(erc => erc.EmergencyRoomCareId == id);
        }

        public async Task<IEnumerable<EmergencyRoomCare>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(erc => erc.Patient)
                .Include(erc => erc.EmergencyRoom)
                    .ThenInclude(er => er.Doctor)
                .ToListAsync();
        }
    }
}