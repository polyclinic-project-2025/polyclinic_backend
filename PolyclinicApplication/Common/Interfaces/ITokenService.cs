using System.Security.Claims;
using PolyclinicApplication.Common.Results;
namespace PolyclinicApplication.Common.Interfaces;

public interface ITokenService
{
    Task<Result<string>> GenerateTokenAsync(
        string userId, 
        string email, 
        IList<string> roles,
        IDictionary<string, string>? additionalClaims = null);
    
    Task<Result<ClaimsPrincipal?>> ValidateTokenAsync(string token);
    
    Task<int> GetTokenExpirationHoursAsync();
}