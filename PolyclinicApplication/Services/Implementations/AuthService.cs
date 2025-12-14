using PolyclinicApplication.DTOs.Response.Auth;
using PolyclinicApplication.DTOs.Request.Auth;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.Common.Interfaces;

namespace PolyclinicApplication.Services.Implementations;


public class AuthService: IAuthService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IRoleValidationService _roleValidationService;
    private readonly ITokenService _tokenService;
    private readonly IEntityLinkingService _entityLinkingService;

    public AuthService(
        IIdentityRepository identityRepository,
        IRoleValidationService roleValidationService,
        ITokenService tokenService,
        IEntityLinkingService entityLinkingService)
    {
        _identityRepository = identityRepository;           
        _roleValidationService = roleValidationService;     
        _tokenService = tokenService;                       
        _entityLinkingService = entityLinkingService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterDto registerDto)
    {
        try
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

            //3. Verificar que las entidades no estén ya vinculadas a un usuario
            var entityNotLinkedResult = await _roleValidationService.ValidateEntityNotLinkedAsync(
                registerDto.Roles,
                registerDto.ValidationData);
            Console.WriteLine("Entity Not Linked Result: ", entityNotLinkedResult);
                
            if (!entityNotLinkedResult.IsSuccess)
            {
                return Result<AuthResponse>.Failure(entityNotLinkedResult.ErrorMessage!);
            }

            // 4. Validar datos requeridos para los roles
            var requiredDataResult = _roleValidationService.ValidateRequiredDataForRoles(
                registerDto.Roles,
                registerDto.ValidationData);
            
            if (!requiredDataResult.Result.IsSuccess)
            {
                return Result<AuthResponse>.Failure(requiredDataResult.Result.ErrorMessage!);
            }

            // 5. Verificar que el usuario no existe
            var userExists = await _identityRepository.UserExistsAsync(registerDto.Email);
            if (userExists)
            {
                return Result<AuthResponse>.Failure("El usuario ya existe.");
            }

            // 6. Crear el usuario
            var createUserResult = await _identityRepository.CreateUserAsync(
                registerDto.Email,
                registerDto.Password,
                registerDto.PhoneNumber);

            if (!createUserResult.IsSuccess)
            {
                return Result<AuthResponse>.Failure(createUserResult.ErrorMessage!);
            }

            var userId = createUserResult.Value!;

            // 7. Asignar roles al usuario
            var assignRolesResult = await _identityRepository.AssignRolesToUserAsync(userId, registerDto.Roles);
            if (!assignRolesResult.IsSuccess)
            {
                return Result<AuthResponse>.Failure(assignRolesResult.ErrorMessage!);
            }

            //8. Vincular entidades al usuario
            foreach (var role in registerDto.Roles)
            {
                var linkEntityResult = await _entityLinkingService.LinkEntityToUserAsync(
                    entityNotLinkedResult.Value!,
                    userId,
                    role);

                if (!linkEntityResult.IsSuccess)
                {
                    return Result<AuthResponse>.Failure(linkEntityResult.ErrorMessage!);
                }
            }

            // 8. Generar token JWT con los roles
            var token = _tokenService.GenerateTokenAsync(
                userId,
                registerDto.Email,
                registerDto.Roles,
                registerDto.ValidationData).Result.Value;

            // 9. Calcular tiempo de expiración
            var expirationHours = _tokenService.GetTokenExpirationHoursAsync().Result;
            var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

            // 10. Retornar respuesta exitosa
            return Result<AuthResponse>.Success(new AuthResponse
            {
                UserId = userId,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Token = token!,
                ExpiresAt = expiresAt,
                Roles = registerDto.Roles,
            });
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Failure($"Error al registrar usuario");
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // 1. Validar credenciales
            var validateResult = await _identityRepository.ValidateCredentialsAsync(
                loginDto.Email,
                loginDto.Password);

            if (!validateResult.IsSuccess)
            {
                return Result<AuthResponse>.Failure("Credenciales inválidas.");
            }

            var userId = validateResult.Value!;

            // 2. Obtener roles del usuario
            var userRoles = await _identityRepository.GetUserRolesAsync(userId);

            // 3. Obtener información del usuario
            var (email, phoneNumber) = await _identityRepository.GetUserInfoAsync(userId);

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
                ExpiresAt = expiresAt,
                Roles = userRoles
            });
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Failure($"Error al iniciar sesión");
        }
    }
}
