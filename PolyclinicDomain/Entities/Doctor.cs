using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Doctor : Employee
{
    public Guid DepartmentId { get; private set; }
    public virtual Department? Department { get; private set; }

    public virtual ICollection<DepartmentHead> DepartmentHeads { get; private set; } = new List<DepartmentHead>();
    public virtual ICollection<EmergencyRoom> EmergencyRooms { get; private set; } = new List<EmergencyRoom>();
    public virtual ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>();
    public virtual ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>();

    public Doctor(Guid doctorId, string identification, string name, string employmentStatus, Guid departmentId)
        : base(doctorId, identification, name, employmentStatus)
    {
        DepartmentId = departmentId;
    }

    protected Doctor() { }

    public override string GetPrimaryRole() => ApplicationRoles.Doctor;

    public void UpdateDepartmentId(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}
