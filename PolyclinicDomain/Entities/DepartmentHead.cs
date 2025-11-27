using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class DepartmentHead : Employee
{
    public Guid DepartmentId { get; private set; }
    
    public Department? Department { get; private set; }
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>(); 
    public ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>(); 
    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>(); 

    public string? UserId { get; set; }

    public DepartmentHead(
        Guid id,
        string name,
        string employmentStatus,
        string identification,
        Guid departmentId) : base(id, name, employmentStatus, identification)
    {
        DepartmentId = departmentId;
    }

    private DepartmentHead() { }

    public override string GetPrimaryRole() => ApplicationRoles.DepartmentHead;

    public void AssignDepartment(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}