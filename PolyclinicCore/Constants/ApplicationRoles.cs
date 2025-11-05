namespace PolyclinicCore.Constants;

/// <summary>
/// Define los roles disponibles en el sistema de la policlínica.
/// Estos roles se utilizan para autorización y control de acceso.
/// </summary>
public static class ApplicationRoles
{
    // ==========================================
    // ROLES ADMINISTRATIVOS
    // ==========================================
    
    /// <summary>
    /// Administrador del sistema con acceso completo
    /// </summary>
    public const string Admin = "Admin";
    
    // ==========================================
    // ROLES MÉDICOS
    // ==========================================
    
    /// <summary>
    /// Doctor con capacidad de diagnóstico y prescripción
    /// </summary>
    public const string Doctor = "Doctor";
    
    /// <summary>
    /// Enfermero/a con capacidad de atención básica
    /// </summary>
    public const string Nurse = "Nurse";
    
    /// <summary>
    /// Personal médico general (incluye doctores, enfermeros, etc)
    /// </summary>
    public const string MedicalStaff = "MedicalStaff";
    
    // ==========================================
    // ROLES OPERATIVOS
    // ==========================================
    
    /// <summary>
    /// Encargado de gestión de almacén
    /// </summary>
    public const string WarehouseManager = "WarehouseManager";
    
    /// <summary>
    /// Jefe de departamento médico
    /// </summary>
    public const string DepartmentHead = "DepartmentHead";
    
    /// <summary>
    /// Personal de sala de emergencias
    /// </summary>
    public const string EmergencyStaff = "EmergencyStaff";
    
    // ==========================================
    // ROLES DE CONSULTA Y PACIENTES
    // ==========================================
    
    /// <summary>
    /// Recepcionista con acceso a registro de pacientes
    /// </summary>
    public const string Receptionist = "Receptionist";
    
    /// <summary>
    /// Paciente con acceso limitado a su información
    /// </summary>
    public const string Patient = "Patient";
    
    // ==========================================
    // COLECCIONES DE ROLES
    // ==========================================
    
    /// <summary>
    /// Todos los roles disponibles en el sistema
    /// </summary>
    public static readonly string[] AllRoles = 
    {
        Admin,
        Doctor,
        Nurse,
        MedicalStaff,
        WarehouseManager,
        DepartmentHead,
        EmergencyStaff,
        Receptionist,
        Patient
    };
    
    /// <summary>
    /// Roles médicos
    /// </summary>
    public static readonly string[] MedicalRoles = 
    {
        Doctor,
        Nurse,
        MedicalStaff,
        EmergencyStaff
    };
    
    /// <summary>
    /// Roles con capacidad de gestión y administración
    /// </summary>
    public static readonly string[] ManagementRoles = 
    {
        Admin,
        DepartmentHead,
        WarehouseManager
    };
    
    /// <summary>
    /// Roles que pueden prescribir medicamentos
    /// </summary>
    public static readonly string[] PrescriptionRoles = 
    {
        Doctor,
        EmergencyStaff
    };
    
    // ==========================================
    // MÉTODOS DE UTILIDAD
    // ==========================================
    
    /// <summary>
    /// Verifica si un rol es válido en el sistema
    /// </summary>
    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
    
    /// <summary>
    /// Verifica si un rol es un rol médico
    /// </summary>
    public static bool IsMedicalRole(string role)
    {
        return MedicalRoles.Contains(role);
    }
    
    /// <summary>
    /// Verifica si un rol es un rol de gestión
    /// </summary>
    public static bool IsManagementRole(string role)
    {
        return ManagementRoles.Contains(role);
    }
    
    /// <summary>
    /// Verifica si un rol puede prescribir medicamentos
    /// </summary>
    public static bool CanPrescribe(string role)
    {
        return PrescriptionRoles.Contains(role);
    }
}