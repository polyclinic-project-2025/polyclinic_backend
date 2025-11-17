using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface IWarehouseManagerService
    {
        Task<Result<WarehouseManagerResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<WarehouseManagerResponseDto>>> GetAllAsync();
        Task<Result<WarehouseManagerResponseDto>> CreateAsync(WarehouseManagerDto dto);
        Task<Result<WarehouseManagerResponseDto>> UpdateAsync(Guid id, WarehouseManagerDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}