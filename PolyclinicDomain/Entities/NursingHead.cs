using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Jefe de Enfermería (Nursing Head).
/// Gestiona un área de enfermería específica.
/// </summary>
public class NursingHead : Employee
{
    public Guid? ManagedNursingId { get; private set; }
    public Nursing? ManagedNursing { get; private set; }

    public NursingHead(Guid id, string name, string employmentStatus, int identification, Guid? managedNursingId = null)
        : base(id, name, employmentStatus, identification)
    {
        ManagedNursingId = managedNursingId;
    }

    // Constructor sin parámetros para EF Core
    private NursingHead() { }

    public override string GetPrimaryRole() => ApplicationRoles.NursingHead;

    public void AssignNursing(Guid nursingId)
    {
        ManagedNursingId = nursingId;
    }

    public void RemoveNursing()
    {
        ManagedNursingId = null;
    }
}