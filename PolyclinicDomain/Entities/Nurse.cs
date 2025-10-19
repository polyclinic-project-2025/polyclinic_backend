using PolyclinicCore;
namespace PolyclinicDomain.Entities;

public class Nurse : Employee
{
    public Nursing? Nursing { get; private set; }
    public Guid? NursingId { get; private set; }
    public Nurse(Guid id, string name, RoleUser role, string employmentStatus, Guid? nursingId) : base(id, name, role, employmentStatus)
    {
        NursingId = nursingId;
    }
}