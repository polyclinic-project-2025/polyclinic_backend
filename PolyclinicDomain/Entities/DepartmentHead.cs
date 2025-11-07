using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Jefe de Departamento.
/// Un empleado puede ser Doctor y DepartmentHead simultáneamente (TPT).
/// </summary>
public class DepartmentHead : Employee
{
    public Guid? ManagedDepartmentId { get; private set; }
    public Department? ManagedDepartment { get; private set; }

    public DepartmentHead(Guid id, string name, string employmentStatus, int identification, Guid? managedDepartmentId = null)
        : base(id, name, employmentStatus, identification)
    {
        ManagedDepartmentId = managedDepartmentId;
    }

    // Constructor sin parámetros para EF Core
    private DepartmentHead() { }

    public override string GetPrimaryRole() => ApplicationRoles.DepartmentHead;

    public void AssignDepartment(Guid departmentId)
    {
        ManagedDepartmentId = departmentId;
    }

    public void RemoveDepartment()
    {
        ManagedDepartmentId = null;
    }
}