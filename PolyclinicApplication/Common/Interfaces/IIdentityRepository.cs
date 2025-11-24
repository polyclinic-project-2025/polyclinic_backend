using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Common.Interfaces;

public interface IIdentityRepository
{
    Task<Result<string>> CreateUserAsync(string email, string password, string? phoneNumber = null);
    
    Task<Result<bool>> AssignRolesToUserAsync(string userId, IList<string> roles);
    
    Task<Result<string>> ValidateCredentialsAsync(string email, string password);
   
    Task<IList<string>> GetUserRolesAsync(string userId);
    
    Task<bool> UserExistsAsync(string email);

    Task<(string Email, string? PhoneNumber)> GetUserInfoAsync(string userId);

    Task<Result<string>> RemoveUserAsync(string email);
    
    Task<Result<string>> UpdateUserValueAsync(string email, string value, string property);
    
    Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync();
    
    // Nuevos métodos para PATCH
    Task<Result<UserResponse>> UpdateUserPropertyAsync(string userId, string propertyName, string value);
    
    Task<Result<UserResponse>> AddRolesToUserAsync(string userId, IList<string> roles);
    
    Task<Result<UserResponse>> RemoveRolesFromUserAsync(string userId, IList<string> roles);
    
    Task<Result<UserResponse>> ReplaceUserRolesAsync(string userId, IList<string> roles);

}