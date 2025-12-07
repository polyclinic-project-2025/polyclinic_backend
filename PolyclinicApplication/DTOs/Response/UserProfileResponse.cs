namespace PolyclinicApplication.DTOs.Response;

/// <summary>
/// DTO unificado para retornar información del perfil vinculado a un usuario.
/// Contiene datos comunes y específicos según el tipo de entidad.
/// </summary>
public class UserProfileResponse
{
    /// <summary>
    /// ID del usuario en Identity
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo de entidad vinculada: "Doctor", "Nurse", "WarehouseManager", "Patient"
    /// </summary>
    public string ProfileType { get; set; } = string.Empty;
    
    /// <summary>
    /// Datos del perfil, el contenido varía según ProfileType
    /// </summary>
    public object Profile { get; set; } = null!;
}

/// <summary>
/// Perfil específico de Doctor
/// </summary>
public class DoctorProfileResponse
{
    public Guid EmployeeId { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    
    // Departamento
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    // Jefatura (si aplica)
    public bool IsDepartmentHead { get; set; }
    public DateTime? DepartmentHeadAssignedAt { get; set; }
}

/// <summary>
/// Perfil específico de Nurse
/// </summary>
public class NurseProfileResponse
{
    public Guid EmployeeId { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
}

/// <summary>
/// Perfil específico de WarehouseManager
/// </summary>
public class WarehouseManagerProfileResponse
{
    public Guid EmployeeId { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;

}

/// <summary>
/// Perfil específico de Patient
/// </summary>
public class PatientProfileResponse
{
    public Guid PatientId { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Contact { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
