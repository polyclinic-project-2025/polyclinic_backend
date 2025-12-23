using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories
{
    public interface IMedicationEmergencyRepository : IRepository<MedicationEmergency>
    {
    Task<IEnumerable<MedicationEmergency>> GetByEmergencyRoomCareIdAsync(Guid emergencyRoomCareId);
    Task<IEnumerable<MedicationEmergency>> GetByMedicationIdAsync(Guid medicationId);
    Task<IEnumerable<MedicationEmergency>> GetByMedicationNameAsync(string medicationName);
    Task<MedicationEmergency?> GetByIdWithMedicationAsync(Guid id);
    Task<IEnumerable<MedicationEmergency>> GetAllWithMedicationAsync();
    }
}