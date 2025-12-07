using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

/// <summary>
/// Implementación del servicio de perfiles de usuario.
/// Mapea los datos del repositorio a DTOs de respuesta.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result<UserProfileResponse>.Failure("El ID de usuario es requerido.");

        var profileData = await _userProfileRepository.GetUserProfileDataAsync(userId);

        if (profileData == null)
            return Result<UserProfileResponse>.Failure("No se encontró ningún perfil vinculado a este usuario.");

        var response = new UserProfileResponse
        {
            UserId = userId,
            ProfileType = profileData.EntityType,
            Profile = MapToSpecificProfile(profileData)
        };

        return Result<UserProfileResponse>.Success(response);
    }

    public async Task<Result<string>> GetLinkedEntityTypeAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result<string>.Failure("El ID de usuario es requerido.");

        var entityType = await _userProfileRepository.GetLinkedEntityTypeAsync(userId);

        if (entityType == null)
            return Result<string>.Failure("No se encontró ninguna entidad vinculada a este usuario.");

        return Result<string>.Success(entityType);
    }

    /// <summary>
    /// Mapea UserProfileData al DTO específico según el tipo de entidad.
    /// </summary>
    private object MapToSpecificProfile(UserProfileData data)
    {
        return data.EntityType switch
        {
            "Doctor" => new DoctorProfileResponse
            {
                EmployeeId = data.EmployeeId!.Value,
                Identification = data.Identification ?? string.Empty,
                Name = data.Name ?? string.Empty,
                EmploymentStatus = data.EmploymentStatus ?? string.Empty,
                DepartmentId = data.DepartmentId!.Value,
                DepartmentName = data.DepartmentName ?? string.Empty,
                IsDepartmentHead = data.IsDepartmentHead,
                DepartmentHeadAssignedAt = data.DepartmentHeadAssignedAt
            },
            
            "Nurse" => new NurseProfileResponse
            {
                EmployeeId = data.EmployeeId!.Value,
                Identification = data.Identification ?? string.Empty,
                Name = data.Name ?? string.Empty,
                EmploymentStatus = data.EmploymentStatus ?? string.Empty
            },
            
            "WarehouseManager" => new WarehouseManagerProfileResponse
            {
                EmployeeId = data.EmployeeId!.Value,
                Identification = data.Identification ?? string.Empty,
                Name = data.Name ?? string.Empty,
                EmploymentStatus = data.EmploymentStatus ?? string.Empty,
                WarehouseId = data.WarehouseId!.Value,
                WarehouseName = data.WarehouseName ?? string.Empty
            },
            
            "Patient" => new PatientProfileResponse
            {
                PatientId = data.PatientId!.Value,
                Identification = data.Identification ?? string.Empty,
                Name = data.Name ?? string.Empty,
                Age = data.Age ?? 0,
                Contact = data.Contact ?? string.Empty,
                Address = data.Address ?? string.Empty
            },
            
            _ => throw new InvalidOperationException($"Tipo de entidad no soportado: {data.EntityType}")
        };
    }
}
