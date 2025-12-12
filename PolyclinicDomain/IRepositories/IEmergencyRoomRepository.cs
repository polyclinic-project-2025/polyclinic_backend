using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;
namespace PolyclinicDomain.IRepositories
{
public interface IEmergencyRoomRepository : IRepository<EmergencyRoom>
{
    Task<IEnumerable<EmergencyRoom>> GetByDateAsync(DateOnly date);
    Task<IEnumerable<EmergencyRoom>> GetByDoctorIdentificationAsync(string doctorIdentification);
    Task<IEnumerable<EmergencyRoom>> GetByDoctorNameAsync(string doctorName);
    Task<EmergencyRoom?> GetByIdWithDoctorAsync(Guid id);
    Task<IEnumerable<EmergencyRoom>> GetAllWithDoctorAsync();
    Task<bool> IsDoctorOnGuardAsync(Guid doctorId, DateOnly date);
}
}