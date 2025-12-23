using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IEmergencyRoomService
    {
        // CRUD (con relaciones)
        Task<Result<EmergencyRoomDto>> CreateAsync(CreateEmergencyRoomDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateEmergencyRoomDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
        
        // Solo métodos con relaciones
        Task<Result<EmergencyRoomDto>> GetByIdWithDoctorAsync(Guid id);
        Task<Result<IEnumerable<EmergencyRoomDto>>> GetAllWithDoctorAsync();
        
        // Métodos de filtrado (ya incluyen relaciones)
        Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDateAsync(DateOnly date);
        Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDoctorIdentificationAsync(string doctorIdentification);
        Task<Result<IEnumerable<EmergencyRoomDto>>> GetByDoctorNameAsync(string doctorName);
    }
}