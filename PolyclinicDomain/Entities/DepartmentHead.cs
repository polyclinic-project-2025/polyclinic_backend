using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class DepartmentHead
{
    public Guid DepartmentHeadId { get; private set; }
    
    public Guid DoctorId { get; private set; }
    public Doctor? Doctor { get; private set; }

    [Required]
    public DateTime AssignedAt { get; private set; }
    
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>(); 
    public ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>(); 
    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>(); 

    public DepartmentHead(Guid departmentHeadId, Guid doctorId, DateTime assignedAt)
    {
        DepartmentHeadId = departmentHeadId;
        DoctorId = doctorId;
        AssignedAt = assignedAt;
    }

    private DepartmentHead() {}

    // public override string GetPrimaryRole() => ApplicationRoles.DepartmentHead;

    public void UpdateHead(Guid doctorId)
    {
        DoctorId = doctorId;
    }
}