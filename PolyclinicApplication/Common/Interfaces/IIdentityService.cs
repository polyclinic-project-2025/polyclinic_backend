using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Common.Interfaces;

/// <summary>
/// Abstracción del sistema de identidad (desacopla de ASP.NET Identity)
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Crea un nuevo usuario en el sistema de identidad
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <param name="password">Contraseña del usuario</param>
    /// <param name="phoneNumber">Número de teléfono opcional</param>
    /// <returns>Result con el ID del usuario creado</returns>
    Task<Result<string>> CreateUserAsync(string email, string password, string? phoneNumber = null);
    
    /// <summary>
    /// Asigna roles a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="roles">Lista de roles a asignar</param>
    /// <returns>Result indicando éxito o fallo</returns>
    Task<Result<bool>> AssignRolesToUserAsync(string userId, IList<string> roles);
    
    /// <summary>
    /// Valida las credenciales de un usuario
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <param name="password">Contraseña del usuario</param>
    /// <returns>Result con el ID del usuario si las credenciales son válidas</returns>
    Task<Result<string>> ValidateCredentialsAsync(string email, string password);
    
    /// <summary>
    /// Obtiene los roles de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de roles del usuario</returns>
    Task<IList<string>> GetUserRolesAsync(string userId);
    
    /// <summary>
    /// Verifica si un usuario existe por email
    /// </summary>
    /// <param name="email">Email a verificar</param>
    /// <returns>True si el usuario existe</returns>
    Task<bool> UserExistsAsync(string email);
    
    /// <summary>
    /// Obtiene información básica del usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Tupla con email y teléfono del usuario</returns>
    Task<(string Email, string? PhoneNumber)> GetUserInfoAsync(string userId);
}