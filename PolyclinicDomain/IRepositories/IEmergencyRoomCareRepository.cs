using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories
{
    public interface IEmergencyRoomCareRepository : IRepository<EmergencyRoomCare>
    {
    Task<IEnumerable<EmergencyRoomCare>> GetByDateAsync(DateTime date);
    Task<IEnumerable<EmergencyRoomCare>> GetByDoctorNameAsync(string doctorName);
    Task<IEnumerable<EmergencyRoomCare>> GetByDoctorIdentificationAsync(string doctorIdentification);
    Task<IEnumerable<EmergencyRoomCare>> GetByPatientNameAsync(string patientName);
    Task<IEnumerable<EmergencyRoomCare>> GetByPatientIdentificationAsync(string patientIdentification);
    Task<EmergencyRoomCare?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<EmergencyRoomCare>> GetAllWithDetailsAsync();
    }
}