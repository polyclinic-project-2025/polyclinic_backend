using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface IMedicalStaffService
    {
        Task<Result<MedicalStaffResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<MedicalStaffResponseDto>>> GetAllAsync();
        Task<Result<MedicalStaffResponseDto>> CreateAsync(MedicalStaffDto dto);
        Task<Result<MedicalStaffResponseDto>> UpdateAsync(Guid id, MedicalStaffDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
