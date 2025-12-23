using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Services.Interfaces;

/// <summary>
/// Servicio para obtener el perfil vinculado a un usuario (empleado o paciente).
/// </summary>
public interface IUserProfileService
{
    /// <summary>
    /// Obtiene el perfil completo vinculado al usuario dado su ID.
    /// Retorna Doctor, Nurse, WarehouseManager o Patient según corresponda.
    /// </summary>
    /// <param name="userId">ID del usuario en Identity</param>
    /// <returns>UserProfileResponse con el tipo y datos del perfil</returns>
    Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
    
    /// <summary>
    /// Obtiene solo el tipo de entidad vinculada al usuario.
    /// Útil para verificaciones rápidas sin cargar todos los datos.
    /// </summary>
    /// <param name="userId">ID del usuario en Identity</param>
    /// <returns>"Doctor", "Nurse", "WarehouseManager", "Patient" o error si no existe</returns>
    Task<Result<string>> GetLinkedEntityTypeAsync(string userId);
}
