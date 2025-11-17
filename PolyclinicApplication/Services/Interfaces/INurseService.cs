using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface INurseService
    {
        Task<Result<NurseResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<NurseResponseDto>>> GetAllAsync();
        Task<Result<NurseResponseDto>> CreateAsync(NurseDto dto);
        Task<Result<NurseResponseDto>> UpdateAsync(Guid id, NurseDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
