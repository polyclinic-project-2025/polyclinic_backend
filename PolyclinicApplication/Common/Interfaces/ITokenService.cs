using System.Security.Claims;
using PolyclinicApplication.Common.Results;
namespace PolyclinicApplication.Common.Interfaces;

/// <summary>
/// Servicio para generación y validación de tokens JWT
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token JWT para un usuario con sus roles
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="email">Email del usuario</param>
    /// <param name="roles">Roles asignados al usuario</param>
    /// <param name="additionalClaims">Claims adicionales opcionales</param>
    /// <returns>Token JWT como string</returns>
    Task<Result<string>> GenerateTokenAsync(
        string userId, 
        string email, 
        IList<string> roles,
        IDictionary<string, string>? additionalClaims = null);
    
    /// <summary>
    /// Valida un token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>ClaimsPrincipal si el token es válido, null si no lo es</returns>
    Task<Result<ClaimsPrincipal?>> ValidateTokenAsync(string token);
    
    /// <summary>
    /// Obtiene el tiempo de expiración del token en horas
    /// </summary>
    Task<int> GetTokenExpirationHoursAsync();
}