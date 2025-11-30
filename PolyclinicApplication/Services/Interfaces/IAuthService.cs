using PolyclinicApplication.DTOs.Request.Auth;
using PolyclinicApplication.DTOs.Response.Auth;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterDto registerDto);
        Task<Result<AuthResponse>> LoginAsync(LoginDto loginDto);
    }
}