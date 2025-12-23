using System;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Services.Interfaces;

public interface IMedicationDerivationService
{
    Task<Result<MedicationDerivationDto>> CreateAsync(CreateMedicationDerivationDto request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDerivationDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<MedicationDerivationDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<MedicationDerivationDto>>> GetAllAsync();
}