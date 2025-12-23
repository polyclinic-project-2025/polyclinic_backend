using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Nurse : Employee
{
    public Nurse(Guid nurseId, string identification, string name, string employmentStatus)
        : base(nurseId, identification, name, employmentStatus) {}

    protected Nurse() { }

    public override string GetPrimaryRole() => ApplicationRoles.Nurse;
}