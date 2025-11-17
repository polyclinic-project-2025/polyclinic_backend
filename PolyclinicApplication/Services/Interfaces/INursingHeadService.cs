using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface INursingHeadService
    {
        Task<Result<NursingHeadResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<NursingHeadResponseDto>>> GetAllAsync();
        Task<Result<NursingHeadResponseDto>> CreateAsync(NursingHeadDto dto);
        Task<Result<NursingHeadResponseDto>> UpdateAsync(Guid id, NursingHeadDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
