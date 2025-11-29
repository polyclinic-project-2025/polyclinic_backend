using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Referral;
using PolyclinicApplication.DTOs.Response.Referral;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PolyclinicApplication.Services.Interfaces{
    public interface IReferralService
{
    Task<ReferralDto> CreateAsync(CreateReferralDto dto);
    Task<IEnumerable<ReferralDto>> GetAllAsync();
    Task<ReferralDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<ReferralDto>> SearchByPuestoExternoAsync(string name);
    Task<IEnumerable<ReferralDto>> SearchByDepartmentToNameAsync(string name);
    Task<IEnumerable<ReferralDto>> SearchByPatientNameAsync(string patientName);
    Task<IEnumerable<ReferralDto>> SearchByDateAsync(DateTime date);
    Task<IEnumerable<ReferralDto>> SearchByPatientIdentificationAsync(string patientIdentification);
    Task DeleteAsync(Guid id);
}

}