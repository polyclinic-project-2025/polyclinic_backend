using PolyclinicCore;
namespace PolyclinicDomain.Entities;

public class MedicalStaff : Employee
{
    public Department? Department { get; private set; }
    public Guid DepartmentId { get; private set; }

    public MedicalStaff(Guid id, string name, RoleUser role, string employmentStatus, Guid departmentId)
        : base(id, name, role, employmentStatus)
    {
        DepartmentId = departmentId;
    }
}
