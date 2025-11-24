using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class UserService : IUserService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRoleValidationService _roleValidationService;

    public UserService(
        IRoleValidationService roleValidationService, 
        IIdentityRepository identityRepository, 
        IEmployeeRepository employeeRepository)
    {
        _roleValidationService = roleValidationService;
        _identityRepository = identityRepository;
        _employeeRepository = employeeRepository;
    }

    public Task<Result<IEnumerable<UserResponse>>> GetAllAsync()
    {
        return _identityRepository.GetAllUsersAsync();
    }

    public Task<Result<string>> RemoveUserAsync(string email)
    {
        return _identityRepository.RemoveUserAsync(email);
    }

    public async Task<Result<UserResponse>> UpdateUserValueAsync(string userId, UpdateUserDto updateUserDto)
    {
        if (string.IsNullOrEmpty(userId))
            return Result<UserResponse>.Failure("El ID del usuario es requerido");

        if (updateUserDto.Operation != null && updateUserDto.Roles != null)
        {
            return await HandleRoleOperation(userId, updateUserDto.Operation, updateUserDto.Roles);
        }

        if (!string.IsNullOrEmpty(updateUserDto.Property) && updateUserDto.Value != null)
        {
            return await UpdateSingleProperty(userId, updateUserDto.Property, updateUserDto.Value);
        }

        return Result<UserResponse>.Failure("Debe proporcionar una operación válida o propiedad con valor");
    }

    private async Task<Result<UserResponse>> HandleRoleOperation(string userId, string operation, IList<string> roles)
    {
        // Validar que el usuario esté vinculado a un empleado
        var employees = await _employeeRepository.FindAsync(e => e.UserId == userId);
        if (!employees.Any())
            return Result<UserResponse>.Failure("El usuario no está vinculado a ningún empleado");

        var employee = employees.First();
        var validationData = new Dictionary<string, string>
        {
            { "IdentificationNumber", employee.Identification}
        };

        // Validar roles requeridos
        var validationResult = await _roleValidationService.ValidateRequiredDataForRoles(roles, validationData);
        if (!validationResult.IsSuccess)
            return Result<UserResponse>.Failure("Error validando roles");

        // Ejecutar operación según el tipo
        Result<UserResponse> result = operation.ToLower() switch
        {
            "add" => await _identityRepository.AddRolesToUserAsync(userId, roles),
            "remove" => await _identityRepository.RemoveRolesFromUserAsync(userId, roles),
            "replace" => await _identityRepository.ReplaceUserRolesAsync(userId, roles),
            _ => Result<UserResponse>.Failure($"Operación '{operation}' no válida. Use: add, remove, o replace")
        };

        return result;
    }

    private async Task<Result<UserResponse>> UpdateSingleProperty(string userId, string propertyName, string value)
    {
        // Validar campos permitidos
        var allowedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Email", "PhoneNumber", "UserName", "EmailConfirmed", "PhoneNumberConfirmed"
        };

        if (!allowedFields.Contains(propertyName))
            return Result<UserResponse>.Failure($"El campo '{propertyName}' no puede ser actualizado");

        // Validaciones específicas por campo
        if (propertyName.Equals("Email", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsValidEmail(value))
                return Result<UserResponse>.Failure("Email no válido");
        }

        if (propertyName.Equals("PhoneNumber", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsValidPhoneNumber(value))
                return Result<UserResponse>.Failure("Número de teléfono no válido");
        }

        return await _identityRepository.UpdateUserPropertyAsync(userId, propertyName, value);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phone)
    {
        return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 7;
    }
}