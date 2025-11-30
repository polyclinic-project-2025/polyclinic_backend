using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<string>> RemoveUserAsync(string email);
        Task<Result<UserResponse>> UpdateUserValueAsync(string userId, UpdateUserDto updateUser);
        Task<Result<IEnumerable<UserResponse>>> GetAllAsync();
    }
}