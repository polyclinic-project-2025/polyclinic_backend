using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace Application.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<DoctorResponseDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<DoctorResponseDto>>> GetAllAsync();
        Task<Result<DoctorResponseDto>> CreateAsync(DoctorDto dto);
        Task<Result<DoctorResponseDto>> UpdateAsync(Guid id, DoctorDto dto);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
