using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs;
using PolyclinicApplication.Common.Results;

public interface IDepartmentHeadService
{
    Task<Result<BossResponse?>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<DepartmentHead>>> GetAllAsync();
    Task<Result<BossResponse>> CreateAsync(BossDto dto, Guid departmentId);
    Task<Result<BossResponse>> UpdateAsync(Guid id, BossDto dto);
    Task<Result<string>> DeleteAsync(Guid id);
}