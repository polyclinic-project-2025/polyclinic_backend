using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IEmergencyRoomCareService
    {
        // CRUD (con relaciones)
        Task<Result<EmergencyRoomCareDto>> CreateAsync(CreateEmergencyRoomCareDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateEmergencyRoomCareDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
        
        // Solo métodos con relaciones
        Task<Result<EmergencyRoomCareDto>> GetByIdWithDetailsAsync(Guid id);
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetAllWithDetailsAsync();
        
        // Métodos de filtrado (ya incluyen relaciones)
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDateAsync(DateTime date);
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDoctorNameAsync(string doctorName);
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByDoctorIdentificationAsync(string doctorIdentification);
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByPatientNameAsync(string patientName);
        Task<Result<IEnumerable<EmergencyRoomCareDto>>> GetByPatientIdentificationAsync(string patientIdentification);
    }
}