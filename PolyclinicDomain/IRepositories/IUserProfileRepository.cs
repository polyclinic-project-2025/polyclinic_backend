using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

/// <summary>
/// Repositorio optimizado para obtener el perfil de usuario vinculado (empleado o paciente)
/// usando consultas eficientes con una sola llamada a la base de datos.
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>
    /// Obtiene el Doctor vinculado al UserId, incluyendo su Department.
    /// Retorna null si no existe.
    /// </summary>
    Task<Doctor?> GetDoctorByUserIdAsync(string userId);

    /// <summary>
    /// Obtiene el Nurse vinculado al UserId.
    /// Retorna null si no existe.
    /// </summary>
    Task<Nurse?> GetNurseByUserIdAsync(string userId);

    /// <summary>
    /// Obtiene el WarehouseManager vinculado al UserId, incluyendo su Warehouse.
    /// Retorna null si no existe.
    /// </summary>
    Task<WarehouseManager?> GetWarehouseManagerByUserIdAsync(string userId);

    /// <summary>
    /// Obtiene el Patient vinculado al UserId.
    /// Retorna null si no existe.
    /// </summary>
    Task<Patient?> GetPatientByUserIdAsync(string userId);

    /// <summary>
    /// Verifica si un Doctor es DepartmentHead y retorna la información.
    /// </summary>
    Task<DepartmentHead?> GetDepartmentHeadByDoctorIdAsync(Guid doctorId);

    /// <summary>
    /// Obtiene el tipo de entidad vinculada al usuario de forma eficiente.
    /// Retorna: "Doctor", "Nurse", "WarehouseManager", "Patient" o null si no existe.
    /// </summary>
    Task<string?> GetLinkedEntityTypeAsync(string userId);

    /// <summary>
    /// Obtiene toda la información del perfil en una sola consulta optimizada.
    /// Incluye: Employee (con su tipo específico) o Patient, y datos adicionales como Department si aplica.
    /// </summary>
    Task<UserProfileData?> GetUserProfileDataAsync(string userId);
}

/// <summary>
/// Clase que agrupa todos los datos del perfil de usuario en una sola estructura.
/// </summary>
public class UserProfileData
{
    public string UserId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // "Doctor", "Nurse", "WarehouseManager", "Patient"
    
    // Datos comunes de empleado (si aplica)
    public Guid? EmployeeId { get; set; }
    public string? Identification { get; set; }
    public string? Name { get; set; }
    public string? EmploymentStatus { get; set; }
    
    // Datos específicos de Doctor
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsDepartmentHead { get; set; }
    public DateTime? DepartmentHeadAssignedAt { get; set; }
    
    // Datos específicos de WarehouseManager
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    
    // Datos de paciente (si aplica)
    public Guid? PatientId { get; set; }
    public int? Age { get; set; }
    public string? Contact { get; set; }
    public string? Address { get; set; }
}
