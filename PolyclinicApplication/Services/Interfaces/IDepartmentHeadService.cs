using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface IDepartmentHeadService
    {
        Task<Result<DepartmentHeadResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<DepartmentHeadResponseDto>>> GetAllAsync();
        Task<Result<DepartmentHeadResponseDto>> CreateAsync(DepartmentHeadDto dto); // ✅ Cambio aquí
        Task<Result<DepartmentHeadResponseDto>> UpdateAsync(Guid id, DepartmentHeadDto dto); // ✅ Cambio aquí
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}