using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Clase base para personal médico que trabaja en un departamento.
/// </summary>
public class MedicalStaff : Employee
{
    public Department? Department { get; private set; }
    public Guid DepartmentId { get; private set; }
    public string? UserId { get; set; }

    public MedicalStaff(Guid id, string name, string employmentStatus, string identification, Guid departmentId)
        : base(id, name, employmentStatus, identification)
    {
        DepartmentId = departmentId;
    }

    // Constructor sin parámetros para EF Core
    protected MedicalStaff() { }

    public override string GetPrimaryRole() => ApplicationRoles.MedicalStaff;
}
