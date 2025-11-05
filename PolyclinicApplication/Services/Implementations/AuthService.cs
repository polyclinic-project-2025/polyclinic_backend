using PolyclinicApplication.DTOs.Response.Auth;
using PolyclinicApplication.DTOs.Request.Auth;
using PolyclinicApplication.Service.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.Common.Interfaces;

namespace PolyclinicApplication.Services.Implementations;

/// <summary>
/// Servicio de autenticación refactorizado siguiendo Clean Architecture
/// Orquesta las operaciones de autenticación usando abstracciones
/// </summary>
public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IRoleValidationService _roleValidationService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IIdentityService identityService,
        IRoleValidationService roleValidationService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _roleValidationService = roleValidationService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterDto registerDto)
    {
        // 1. Validar que los roles existen en el sistema
        var rolesExistResult = await _roleValidationService.ValidateRolesExistAsync(registerDto.Roles);
        if (!rolesExistResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(rolesExistResult.ErrorMessage!);
        }

        // 2. Validar combinación de roles según reglas de negocio
        var rolesCombinationResult = _roleValidationService.ValidateRolesCombination(
            registerDto.Roles,
            registerDto.ValidationData);
        
        if (!rolesCombinationResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(rolesCombinationResult.ErrorMessage!);
        }

        // 3. Validar datos requeridos para los roles
        var requiredDataResult = _roleValidationService.ValidateRequiredDataForRoles(
            registerDto.Roles,
            registerDto.ValidationData);
        
        if (!requiredDataResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(requiredDataResult.ErrorMessage!);
        }

        // 4. Verificar que el usuario no existe
        var userExists = await _identityService.UserExistsAsync(registerDto.Email);
        if (userExists)
        {
            return Result<AuthResponse>.Failure("El usuario ya existe.");
        }

        // 5. Crear el usuario
        var createUserResult = await _identityService.CreateUserAsync(
            registerDto.Email,
            registerDto.Password,
            registerDto.PhoneNumber);

        if (!createUserResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(createUserResult.ErrorMessage!);
        }

        var userId = createUserResult.Value!;

        // 6. Asignar roles al usuario
        var assignRolesResult = await _identityService.AssignRolesToUserAsync(userId, registerDto.Roles);
        if (!assignRolesResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure(assignRolesResult.ErrorMessage!);
        }

        // 7. Generar token JWT con los roles
        var token = _tokenService.GenerateTokenAsync(
            userId,
            registerDto.Email,
            registerDto.Roles,
            registerDto.ValidationData).Result.Value;

        // 8. Calcular tiempo de expiración
        var expirationHours = _tokenService.GetTokenExpirationHoursAsync().Result;
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        // 9. Retornar respuesta exitosa
        return Result<AuthResponse>.Success(new AuthResponse
        {
            UserId = userId,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            Token = token!,
            ExpiresAt = expiresAt
        });
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginDto loginDto)
    {
        // 1. Validar credenciales
        var validateResult = await _identityService.ValidateCredentialsAsync(
            loginDto.Email,
            loginDto.Password);

        if (!validateResult.IsSuccess)
        {
            return Result<AuthResponse>.Failure("Credenciales inválidas.");
        }

        var userId = validateResult.Value!;

        // 2. Obtener roles del usuario
        var userRoles = await _identityService.GetUserRolesAsync(userId);

        // 3. Obtener información del usuario
        var (email, phoneNumber) = await _identityService.GetUserInfoAsync(userId);

        // 4. Generar token JWT con los roles
        var token = _tokenService.GenerateTokenAsync(userId, email, userRoles).Result.Value;

        // 5. Calcular tiempo de expiración
        var expirationHours = _tokenService.GetTokenExpirationHoursAsync().Result;
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        // 6. Retornar respuesta exitosa
        return Result<AuthResponse>.Success(new AuthResponse
        {
            UserId = userId,
            Email = email,
            PhoneNumber = phoneNumber,
            Token = token!,
            ExpiresAt = expiresAt
        });
    }
}
