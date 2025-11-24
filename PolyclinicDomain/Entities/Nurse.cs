using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Enfermero/a en el sistema.
/// </summary>
public class Nurse : Employee
{
    public Nursing? Nursing { get; private set; }
    public Guid NursingId { get; private set; }
    public Nurse(Guid id, string identification, string name, string employmentStatus, Guid nursingId)
        : base(id, identification, name, employmentStatus)
    {
        NursingId = nursingId;
    }

    // Constructor sin parámetros para EF Core
    private Nurse() { }

    public override string GetPrimaryRole() => ApplicationRoles.Nurse;
}