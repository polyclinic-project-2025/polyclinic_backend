using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IMedicationEmergencyService
    {
        // CRUD (con relaciones)
        Task<Result<MedicationEmergencyDto>> CreateAsync(CreateMedicationEmergencyDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationEmergencyDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
        
        // Solo métodos con relaciones
        Task<Result<MedicationEmergencyDto>> GetByIdWithMedicationAsync(Guid id);
        Task<Result<IEnumerable<MedicationEmergencyDto>>> GetAllWithMedicationAsync();
        
        // Métodos de filtrado (ya incluyen relaciones)
        Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByEmergencyRoomCareIdAsync(Guid emergencyRoomCareId);
        Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByMedicationIdAsync(Guid medicationId);
        Task<Result<IEnumerable<MedicationEmergencyDto>>> GetByMedicationNameAsync(string medicationName);
    }
}