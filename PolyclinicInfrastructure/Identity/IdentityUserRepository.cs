using Microsoft.AspNetCore.Identity;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicInfrastructure.Identity;

public class IdentityUserRepository : IIdentityRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityUserRepository(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<Result<string>> CreateUserAsync(string email, string password, string? phoneNumber = null)
    {
        var newUser = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = email,
            Email = email,
            PhoneNumber = phoneNumber,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            return Result<string>.Failure("Error al crear el usuario");
        }

        return Result<string>.Success(newUser.Id);
    }

    public async Task<Result<bool>> AssignRolesToUserAsync(string userId, IList<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<bool>.Failure("Usuario no encontrado.");
        }

        foreach (var role in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return Result<bool>.Failure($"El rol '{role}' no existe en el sistema.");
            }
        }

        var result = await _userManager.AddToRolesAsync(user, roles);

        if (!result.Succeeded)
        {
            return Result<bool>.Failure("Error al asignar roles");
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<string>.Failure("Credenciales inválidas.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Result<string>.Failure("Credenciales inválidas.");
        }

        return Result<string>.Success(user.Id);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<(string Email, string? PhoneNumber)> GetUserInfoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (string.Empty, null);
        }

        return (user.Email ?? string.Empty, user.PhoneNumber);
    }

    public async Task<Result<string>> RemoveUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            return Result<string>.Success(email);
        }
        return Result<string>.Failure($"El usuario {email} no existe");
    }

    public async Task<Result<string>> UpdateUserValueAsync(string userId, string value, string propertyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var property = typeof(IdentityUser).GetProperty(propertyName);
            if (property == null || !property.CanWrite) 
                return Result<string>.Failure("No se encuentra el campo solicitado o no puede sobreescribirse");
            
            property.SetValue(user, value);
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded) 
                return Result<string>.Success($"El usuario {userId} ha sido actualizado correctamente");
            
            return Result<string>.Failure($"Error al actualizar usuario");
        }
        return Result<string>.Failure($"Usuario {userId} no encontrado");
    }

    public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserResponse>();
        
        foreach (var user in users)
        {
            var userResponse = new UserResponse
            {
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Roles = await GetUserRolesAsync(user.Id)
            };
            
            result.Add(userResponse);
        }
        return Result<IEnumerable<UserResponse>>.Success(result);
    }


    public async Task<Result<UserResponse>> UpdateUserPropertyAsync(string userId, string propertyName, string value)
    {
        var user =  await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserResponse>.Failure("Usuario no encontrado");

        try
        {
            switch (propertyName.ToLower())
            {
                case "email":
                    user.Email = value;
                    user.NormalizedEmail = value.ToUpperInvariant();
                    break;
                case "phonenumber":
                    user.PhoneNumber = value;
                    break;
                case "username":
                    user.UserName = value;
                    user.NormalizedUserName = value.ToUpperInvariant();
                    break;
                case "emailconfirmed":
                    user.EmailConfirmed = bool.Parse(value);
                    break;
                case "phonenumberconfirmed":
                    user.PhoneNumberConfirmed = bool.Parse(value);
                    break;
                default:
                    return Result<UserResponse>.Failure($"Propiedad '{propertyName}' no válida");
            }

            user.SecurityStamp = Guid.NewGuid().ToString();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<UserResponse>.Failure($"Error al actualizar: {errors}");
            }

            var userResponse = await MapToUserResponseAsync(user);
            return Result<UserResponse>.Success(userResponse);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Error al actualizar propiedad: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> AddRolesToUserAsync(string userId, IList<string> roles)
    {
        var user =  await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserResponse>.Failure("Usuario no encontrado");

        // Verificar que los roles existan
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return Result<UserResponse>.Failure($"El rol '{role}' no existe");
        }

        // Obtener roles actuales
        var currentRoles = await _userManager.GetRolesAsync(user);
        
        // Agregar solo los roles que no tiene
        var rolesToAdd = roles.Except(currentRoles).ToList();
        
        if (rolesToAdd.Any())
        {
            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<UserResponse>.Failure($"Error al agregar roles: {errors}");
            }
        }

        var userResponse = await MapToUserResponseAsync(user);
        return Result<UserResponse>.Success(userResponse);
    }

    public async Task<Result<UserResponse>> RemoveRolesFromUserAsync(string userId, IList<string> roles)
    {
        var user =  await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserResponse>.Failure("Usuario no encontrado");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = roles.Intersect(currentRoles).ToList();

        if (rolesToRemove.Any())
        {
            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<UserResponse>.Failure($"Error al remover roles: {errors}");
            }
        }

        var userResponse = await MapToUserResponseAsync(user);
        return Result<UserResponse>.Success(userResponse);
    }

    public async Task<Result<UserResponse>> ReplaceUserRolesAsync(string userId, IList<string> roles)
    {
        var user =  await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserResponse>.Failure("Usuario no encontrado");

        // Verificar que todos los roles existan
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return Result<UserResponse>.Failure($"El rol '{role}' no existe");
        }

        // Remover todos los roles actuales
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return Result<UserResponse>.Failure($"Error al remover roles existentes: {errors}");
            }
        }

        // Agregar los nuevos roles
        if (roles.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                return Result<UserResponse>.Failure($"Error al asignar nuevos roles: {errors}");
            }
        }

        var userResponse = await MapToUserResponseAsync(user);
        return Result<UserResponse>.Success(userResponse);
    }

    private async Task<UserResponse> MapToUserResponseAsync(IdentityUser user)
    {
        return new UserResponse
        {
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Roles = await GetUserRolesAsync(user.Id)
        };
    }
}