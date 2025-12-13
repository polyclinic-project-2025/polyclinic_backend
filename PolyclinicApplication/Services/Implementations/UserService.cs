using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;

namespace PolyclinicApplication.Services.Implementations;

public class UserService : IUserService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IRoleValidationService _roleValidationService;
    private readonly IRepository<Patient> _patientRepository;

    public UserService(
        IRoleValidationService roleValidationService, 
        IIdentityRepository identityRepository,
        IRepository<Patient> patientRepository, 
        IRepository<Employee> employeeRepository)
    {
        _roleValidationService = roleValidationService;
        _identityRepository = identityRepository;
        _employeeRepository = employeeRepository;
        _patientRepository = patientRepository;
    }

    public Task<Result<IEnumerable<UserResponse>>> GetAllAsync()
    {
        return _identityRepository.GetAllUsersAsync();
    }

    public Task<Result<string>> RemoveUserAsync(string userId)
    {
        return _identityRepository.RemoveUserAsync(userId);
    }

    public async Task<Result<UserResponse>> UpdateUserValueAsync(string userId, UpdateUserDto updateUserDto)
    {
        try
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
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Error al actualizar usuario: {ex.Message}");
        }
    }

    private async Task<Result<UserResponse>> HandleRoleOperation(string userId, string operation, IList<string> roles)
    {
        try
        {
            // Validar que el usuario esté vinculado a un empleado
            var employees = await _employeeRepository.FindAsync(e => e.UserId == userId);
            var patients = await _patientRepository.FindAsync(e => e.UserId == userId);
            if (!employees.Any() && !patients.Any())
                return Result<UserResponse>.Failure("El usuario no está vinculado a ningún empleado o paciente");

            var Identification = employees.Any() ? employees.First().Identification : patients.First().Identification;
            var validationData = new Dictionary<string, string>
            {
                { "IdentificationNumber", Identification}
            };

            // Validar roles requeridos
            var validationResult = await _roleValidationService.ValidateRequiredDataForRoles(roles, validationData);
            if (!validationResult.IsSuccess)
                return Result<UserResponse>.Failure(validationResult.ErrorMessage ?? "Error en la validación de roles");

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
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Error al manejar operación de roles: {ex.Message}");
        }
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