using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Enfermero/a en el sistema.
/// </summary>
public class Nurse : Employee
{
    public Nursing? Nursing { get; private set; }
    public Guid NursingId { get; private set; }
    public string? UserId { get; set; }

    public Nurse(Guid id, string name, string employmentStatus, string identification, Guid nursingId)
        : base(id, name, employmentStatus, identification)
    {
        NursingId = nursingId;
    }

    // Constructor sin parámetros para EF Core
    private Nurse() { }

    public override string GetPrimaryRole() => ApplicationRoles.Nurse;
}