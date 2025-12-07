using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.DTOs.Response.MedicationReferrals;

namespace PolyclinicApplication.Services.Interfaces;

public interface IMedicationReferralService
{
    Task<Result<MedicationReferralDto>> CreateAsync(CreateMedicationReferralDto request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationReferralDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<MedicationReferralDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<MedicationReferralDto>>> GetAllAsync();
}
