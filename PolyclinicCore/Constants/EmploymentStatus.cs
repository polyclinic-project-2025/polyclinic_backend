namespace PolyclinicCore.Constants;

public static class EmploymentStatus
{
    public const string Active = "Activo";
    public const string Inactive = "Inactivo";
    public const string LeaveOfAbsence = "Baja Temporal";
    public const string Exemployee = "Exempleado";

    public static readonly string[] AllEmploymentStatuses =
    {
        Active,
        Inactive,
        LeaveOfAbsence,
        Exemployee
    };
    
    public static bool IsValidEmploymentStatus(string employmentStatus)
    {
        return AllEmploymentStatuses.Contains(employmentStatus);
    }
}