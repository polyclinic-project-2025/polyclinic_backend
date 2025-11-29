using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IPatientService
    {
        // CREATE
        Task<PatientDto> CreateAsync(CreatePatientDto dto);

        // READ
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<PatientDto?> GetByIdAsync(Guid id);
        Task<PatientDto?> GetByIdentificationAsync(string identification);
        Task<IEnumerable<PatientDto>> GetByNameAsync(string name);
        Task<IEnumerable<PatientDto>> GetByAgeAsync(int age);
        //Task<Patient> GetWithRelationsAsync(Guid id);

        // UPDATE
        Task UpdateAsync(Guid id, UpdatePatientDto dto);

        // DELETE
        Task DeleteAsync(Guid id);
    }
}
