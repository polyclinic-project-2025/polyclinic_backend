using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Interfaces;

public interface IMedicationRequestService
{
    Task<Result<IEnumerable<MedicationRequestResponse>>> GetAllMedicationRequestAsync();
    Task<Result<MedicationRequestResponse>> GetMedicationRequestByIdAsync(Guid id);
    Task<Result<IEnumerable<MedicationRequestResponse>>> GetMedicationRequestByWarehouseRequestIdAsync(Guid warehouseRequestId);
    Task<Result<MedicationRequestResponse>> CreateMedicationRequestAsync(CreateMedicationRequestRequest request);
    Task<Result<bool>> UpdateMedicationRequestAsync(Guid id, UpdateMedicationRequestRequest request);
    Task<Result<bool>> DeleteMedicationRequestAsync(Guid id);
}