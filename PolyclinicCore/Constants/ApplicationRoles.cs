namespace PolyclinicCore.Constants;

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
    public const string Nurse = "Enfermero";
    
    /// <summary>
    /// Personal médico general (incluye doctores, enfermeros, etc)
    /// </summary>
    public const string MedicalStaff = "Personal Médico";
    
    // ==========================================
    // ROLES OPERATIVOS
    // ==========================================
    
    /// <summary>
    /// Encargado de gestión de almacén
    /// </summary>
    public const string WarehouseManager = "Jefe de Almacén";

    /// <summary>
    /// Jefe de departamento médico
    /// </summary>
    public const string DepartmentHead = "Jefe de Departamente";

    // ==========================================
    // ROLES DE CONSULTA Y PACIENTES
    // ==========================================

    /// <summary>
    /// Paciente con acceso limitado a su información
    /// </summary>
    public const string Patient = "Patient";
    
    // ==========================================
    // COLECCIONES DE ROLES
    // ==========================================
    
    public static readonly string[] AllRoles =
    {
        Admin,
        Doctor,
        Nurse,
        MedicalStaff,
        WarehouseManager,
        DepartmentHead,
        Patient,

    };
    
    public static readonly string[] MedicalRoles = 
    {
        Doctor,
        Nurse,
        MedicalStaff
    };
    
    public static readonly string[] ManagementRoles =
    {
        Admin,
        DepartmentHead,
        WarehouseManager
    };
    
    public static readonly string[] PrescriptionRoles = 
    {
        Doctor
    };
    
    // ==========================================
    // MÉTODOS DE UTILIDAD
    // ==========================================
    
    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
    
    public static bool IsMedicalRole(string role)
    {
        return MedicalRoles.Contains(role);
    }
    
    public static bool IsManagementRole(string role)
    {
        return ManagementRoles.Contains(role);
    }
    
    public static bool CanPrescribe(string role)
    {
        return PrescriptionRoles.Contains(role);
    }
}