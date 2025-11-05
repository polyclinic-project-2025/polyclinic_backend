using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Common.Interfaces;

/// <summary>
/// Servicio para validación de roles y reglas de negocio relacionadas
/// </summary>
public interface IRoleValidationService
{
    /// <summary>
    /// Valida que todos los roles proporcionados existan en el sistema
    /// </summary>
    /// <param name="roles">Lista de roles a validar</param>
    /// <returns>Result indicando si todos los roles son válidos</returns>
    Task<Result<bool>> ValidateRolesExistAsync(IList<string> roles);
    
    /// <summary>
    /// Valida las reglas de negocio para la combinación de roles
    /// Por ejemplo: un usuario no puede ser Doctor y Nurse simultáneamente
    /// </summary>
    /// <param name="roles">Lista de roles a validar</param>
    /// <param name="validationData">Datos adicionales para validación (ej: departamento, credenciales)</param>
    /// <returns>Result indicando si la combinación de roles es válida</returns>
    Result<bool> ValidateRolesCombination(IList<string> roles, Dictionary<string, string>? validationData = null);
    
    /// <summary>
    /// Valida que el usuario tenga los datos requeridos para los roles asignados
    /// Por ejemplo: un Doctor debe tener número de licencia médica
    /// </summary>
    /// <param name="roles">Lista de roles</param>
    /// <param name="validationData">Datos del usuario para validar</param>
    /// <returns>Result indicando si los datos son suficientes</returns>
    Result<bool> ValidateRequiredDataForRoles(IList<string> roles, Dictionary<string, string>? validationData);
}