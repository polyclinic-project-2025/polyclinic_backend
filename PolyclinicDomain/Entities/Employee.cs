namespace PolyclinicDomain.Entities;

/// <summary>
/// Clase base para todos los empleados del sistema.
/// Usa Table-Per-Type (TPT) para permitir múltiples roles.
/// </summary>
public abstract class Employee
{
    public Guid Id { get; private set; }
    public int Identification { get; private set; }
    public string? Name { get; private set; }
    public string? EmploymentStatus { get; private set; }

    protected Employee(Guid id, string name, string employmentStatus, int identification)
    {
        Id = id;
        Name = name;
        EmploymentStatus = employmentStatus;
        Identification = identification;
    }

    // Constructor sin parámetros para EF Core
    protected Employee() { }

    /// <summary>
    /// Obtiene el rol principal basado en el tipo de empleado
    /// </summary>
    public abstract string GetPrimaryRole();
}