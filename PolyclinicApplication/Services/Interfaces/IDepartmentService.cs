using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateDepartmentDto dto);
        Task DeleteAsync(Guid id);
    }
}
