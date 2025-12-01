using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IPatientService
    {
        // CREATE
        Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto);

        // READ
        Task<Result<IEnumerable<PatientDto>>> GetAllAsync();
        Task<Result<PatientDto>> GetByIdAsync(Guid id);
        Task<Result<PatientDto>> GetByIdentificationAsync(string identification);
        Task<Result<IEnumerable<PatientDto>>> GetByNameAsync(string name);
        Task<Result<IEnumerable<PatientDto>>> GetByAgeAsync(int age);

        // UPDATE
        Task<Result<bool>> UpdateAsync(Guid id, UpdatePatientDto dto);

        // DELETE
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
