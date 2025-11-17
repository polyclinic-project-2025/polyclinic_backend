using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Common.Interfaces;

public interface IRoleValidationService
{
    Task<Result<bool>> ValidateRolesExistAsync(IList<string> roles);

    Result<bool> ValidateRolesCombination(IList<string> roles, Dictionary<string, string>? validationData = null);

    Task<Result<bool>> ValidateRequiredDataForRoles(IList<string> roles, Dictionary<string, string>? validationData);

    Task<Result<Guid>> ValidateEntityNotLinkedAsync(IList<string> roles, Dictionary<string, string>? validationData);
}