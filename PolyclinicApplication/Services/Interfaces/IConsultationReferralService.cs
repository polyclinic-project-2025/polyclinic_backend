using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicApplication.Common.Results;


namespace PolyclinicApplication.Services.Interfaces;

public interface IConsultationReferralService
{
    Task<Result<IEnumerable<ConsultationReferralResponse>>> GetAllAsync();

    Task<Result<ConsultationReferralResponse>> GetByIdAsync(Guid id);

    Task<Result<ConsultationReferralResponse>> CreateAsync(CreateConsultationReferralDto request);

    Task<Result<ConsultationReferralResponse>> UpdateAsync(Guid id, UpdateConsultationReferralDto request);
    
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<IEnumerable<ConsultationReferralResponse>>> GetLastTen();
    Task<Result<IEnumerable<ConsultationReferralResponse>>> GetConsultationInRange(DateTime start, DateTime end);

}

