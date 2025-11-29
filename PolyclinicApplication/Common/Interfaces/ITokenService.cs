using System.Security.Claims;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;
using System.IdentityModel.Tokens.Jwt;

namespace PolyclinicApplication.Common.Interfaces;

public interface ITokenService
{
    Task<Result<string>> GenerateTokenAsync(
        string userId, 
        string email, 
        IList<string> roles,
        IDictionary<string, string>? additionalClaims = null);

    Task<Result<ClaimsPrincipal?>> ValidateTokenAsync(string token);
    Task<Result<UserResponse>> DecodingAuthAsync(string authHeader);
    Task<Result<JwtSecurityToken>> DecodeJwtTokenAsync(string authHeader, bool bearer = true);
    Task<int> GetTokenExpirationHoursAsync();
}