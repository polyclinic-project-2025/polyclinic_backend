using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicCore.Constants;

namespace PolyclinicApplication.Services.Implementations;

public class RoleValidationService : IRoleValidationService
{
    private readonly IIdentityService _identityService;

    public RoleValidationService(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<bool>> ValidateRolesExistAsync(IList<string> roles)
    {
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        var invalidRoles = new List<string>();

        foreach (var role in roles)
        {
            if (!ApplicationRoles.IsValidRole(role))
            {
                invalidRoles.Add(role);
            }
        }

        if (invalidRoles.Any())
        {
            return Result<bool>.Failure(
                $"Los siguientes roles no son válidos: {string.Join(", ", invalidRoles)}");
        }

        if (roles.Contains(ApplicationRoles.Admin)) return Result<bool>.Failure("No se permite el registro de administrador");

        return Result<bool>.Success(true);
    }

    public Result<bool> ValidateRolesCombination(IList<string> roles, Dictionary<string, string>? validationData = null)
    {
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        // Regla 1: Un usuario no puede ser Doctor y Nurse simultáneamente
        if (roles.Contains(ApplicationRoles.Doctor) && roles.Contains(ApplicationRoles.Nurse))
        {
            return Result<bool>.Failure(
                "Un usuario no puede tener los roles de Doctor y Nurse simultáneamente.");
        }

        // Regla 2: MedicalStaff es un rol genérico, no debe combinarse con Doctor o Nurse
        if (roles.Contains(ApplicationRoles.MedicalStaff))
        {
            if (roles.Contains(ApplicationRoles.Doctor) || roles.Contains(ApplicationRoles.Nurse))
            {
                return Result<bool>.Failure(
                    "El rol MedicalStaff no debe combinarse con Doctor o Nurse. Use roles específicos.");
            }
        }

        return Result<bool>.Success(true);
    }

    public Result<bool> ValidateRequiredDataForRoles(IList<string> roles, Dictionary<string, string>? validationData)
    {
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        if (validationData == null)
        {
            validationData = new Dictionary<string, string>();
        }

        var missingData = new List<string>();
        //se deberia crear un validador segun la data necesaria para el rol
        // Validar datos requeridos para roles
        if (ApplicationRoles.MedicalRoles.Any(mr => roles.Contains(mr)))
        {
            if (!validationData.ContainsKey("IdentificationNumber"))
            {
                missingData.Add("Número de identificación: (IdentificationNumber)");
            }
        }

        if (missingData.Any())
        {
            return Result<bool>.Failure(
                $"Faltan los siguientes datos requeridos: {string.Join(", ", missingData)}");
        }

        return Result<bool>.Success(true);
    }
}