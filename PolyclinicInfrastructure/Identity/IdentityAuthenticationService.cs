using Microsoft.AspNetCore.Identity;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;

namespace PolyclinicInfrastructure.Identity;

/// <summary>
/// Implementación del servicio de identidad usando ASP.NET Core Identity
/// Esta clase encapsula toda la interacción con Identity
/// </summary>
public class IdentityAuthenticationService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityAuthenticationService(
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
            EmailConfirmed = false // En producción, implementar confirmación por email
        };

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Error al crear el usuario: {errors}");
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

        // Verificar que todos los roles existen
        foreach (var role in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return Result<bool>.Failure($"El rol '{role}' no existe en el sistema.");
            }
        }

        // Asignar roles
        var result = await _userManager.AddToRolesAsync(user, roles);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.Failure($"Error al asignar roles: {errors}");
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
}