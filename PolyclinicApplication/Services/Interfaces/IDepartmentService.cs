using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
        Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync();
        Task<Result<DepartmentDto?>> GetByIdAsync(Guid id);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateDepartmentDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);

        Task<Result<List<DoctorResponse>>> GetDoctorsByDepartmentIdAsync(Guid departmentId);
    }
}
