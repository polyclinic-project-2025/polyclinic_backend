using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface IDepartmentHeadService
    {
        Task<Result<DepartmentHeadDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<DepartmentHeadDto>>> GetAllAsync();
        Task<Result<DepartmentHeadDto>> CreateAsync(CreateDepartmentHeadDto dto); // ✅ Cambio aquí
        Task<Result<DepartmentHeadDto>> UpdateAsync(Guid id, CreateDepartmentHeadDto dto); // ✅ Cambio aquí
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}