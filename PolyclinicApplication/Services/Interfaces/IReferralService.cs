using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Referral;
using PolyclinicApplication.DTOs.Response.Referral;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
namespace PolyclinicApplication.Services.Interfaces{
    public interface IReferralService
{
    Task<Result<ReferralDto>> CreateAsync(CreateReferralDto dto);
    Task<Result<IEnumerable<ReferralDto>>> GetAllAsync();
    Task<Result<ReferralDto>> GetByIdAsync(Guid id);

    Task<Result<IEnumerable<ReferralDto>>> SearchByPuestoExternoAsync(string name);
    Task<Result<IEnumerable<ReferralDto>>> SearchByDepartmentToNameAsync(string name);
    Task<Result<IEnumerable<ReferralDto>>> SearchByPatientNameAsync(string patientName);
    Task<Result<IEnumerable<ReferralDto>>> SearchByDateAsync(DateTime date);
    Task<Result<IEnumerable<ReferralDto>>> SearchByPatientIdentificationAsync(string patientIdentification);
    Task<Result<bool>> DeleteAsync(Guid id);
}

}