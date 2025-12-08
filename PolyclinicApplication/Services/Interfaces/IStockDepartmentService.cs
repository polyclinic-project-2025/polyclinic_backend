using PolyclinicApplication.DTOs.Request.StockDepartment;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IStockDepartmentService
    {
        //CRUD
        Task<Result<StockDepartmentDto>> CreateAsync(CreateStockDepartmentDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateStockDepartmentDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<StockDepartmentDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<StockDepartmentDto>>> GetAllAsync();

        //Custom
        Task<Result<IEnumerable<StockDepartmentDto>>> GetStockByDepartmentIdAsync(Guid departmentId);
        Task<Result<IEnumerable<StockDepartmentDto>>> GetLowStockByDepartmentIdAsync(Guid departmentId);
        Task<Result<IEnumerable<StockDepartmentDto>>> GetOverStockByDepartmentIdAsync(Guid departmentId);
    }
}