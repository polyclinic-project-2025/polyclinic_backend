using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Nurse : Employee
{
    public Guid NursingId { get; private set; }
    public Nursing? Nursing { get; private set; }

    public Nurse(Guid nurseId, string identification, string name, string employmentStatus, Guid nursingId)
        : base(nurseId, identification, name, employmentStatus)
    {
        NursingId = nursingId;
    }

    private Nurse() { }

    public override string GetPrimaryRole() => ApplicationRoles.Nurse;
}