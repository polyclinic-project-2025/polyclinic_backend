using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
namespace PolyclinicApplication.Services.Interfaces{
    public interface IDerivationService
{
    Task<Result<DerivationDto>> CreateAsync(CreateDerivationDto dto);
    Task<Result<IEnumerable<DerivationDto>>> GetAllAsync();
    Task<Result<DerivationDto>> GetByIdAsync(Guid id);

    Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentFromNameAsync(string name);
    Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentToNameAsync(string name);
    Task<Result<IEnumerable<DerivationDto>>> SearchByPatientNameAsync(string patientName);
    Task<Result<IEnumerable<DerivationDto>>> SearchByDateAsync(DateTime date);
    Task<Result<IEnumerable<DerivationDto>>> SearchByPatientIdentificationAsync(string patientIdentification);
    Task<Result<bool>> DeleteAsync(Guid id);
}

}
