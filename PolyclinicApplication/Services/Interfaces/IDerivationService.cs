using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PolyclinicApplication.Services.Interfaces{
    public interface IDerivationService
{
    Task<DerivationDto> CreateAsync(CreateDerivationDto dto);
    Task<IEnumerable<DerivationDto>> GetAllAsync();
    Task<DerivationDto?> GetByIdAsync(Guid id);

    Task<IEnumerable<DerivationDto>> SearchByDepartmentFromNameAsync(string name);
    Task<IEnumerable<DerivationDto>> SearchByDepartmentToNameAsync(string name);
    Task<IEnumerable<DerivationDto>> SearchByPatientNameAsync(string patientName);
    Task<IEnumerable<DerivationDto>> SearchByDateAsync(DateTime date);
    Task<IEnumerable<DerivationDto>> SearchByPatientIdentificationAsync(string patientIdentification);
    Task DeleteAsync(Guid id);
}

}
