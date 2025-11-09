using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Common.Interfaces;


public interface IIdentityService
{
    Task<Result<string>> CreateUserAsync(string email, string password, string? phoneNumber = null);
    
    Task<Result<bool>> AssignRolesToUserAsync(string userId, IList<string> roles);
    
    Task<Result<string>> ValidateCredentialsAsync(string email, string password);
   
    Task<IList<string>> GetUserRolesAsync(string userId);
    
    Task<bool> UserExistsAsync(string email);

    Task<(string Email, string? PhoneNumber)> GetUserInfoAsync(string userId);
}