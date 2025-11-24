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
    /// Jefe de enfermería
    /// </summary>
    public const string NursingHead = "NursingHead";
    
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
        Patient,
        NursingHead
    };
    
    public static readonly string[] MedicalRoles = 
    {
        Doctor,
        Nurse,
        MedicalStaff,
        EmergencyStaff
    };
    
    public static readonly string[] ManagementRoles =
    {
        Admin,
        DepartmentHead,
        WarehouseManager,
        NursingHead
    };
    
    public static readonly string[] PrescriptionRoles = 
    {
        Doctor,
        EmergencyStaff
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