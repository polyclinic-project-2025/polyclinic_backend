using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class MedicalStaff : Employee
{
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public MedicalStaff(Guid medicalStaffId, string name, string employmentStatus, string identification, Guid departmentId)
        : base(medicalStaffId, name, employmentStatus, identification)
    {
        DepartmentId = departmentId;
    }

    protected MedicalStaff() { }

    public override string GetPrimaryRole() => ApplicationRoles.MedicalStaff;

    public void UpdateDepartmentId(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}
