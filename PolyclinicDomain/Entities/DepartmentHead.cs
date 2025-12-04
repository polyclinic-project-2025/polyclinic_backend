using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class DepartmentHead
{
    public Guid DepartmentHeadId { get; private set; }
    
    public Guid DoctorId { get; private set; }
    public virtual Doctor? Doctor { get; private set; }

    public Guid DepartmentId { get; private set; }
    public virtual Department? Department { get; private set; }

    [Required]
    public DateTime AssignedAt { get; private set; }
    
    public virtual ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>(); 
    public virtual ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>(); 
    public virtual ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>(); 

    public DepartmentHead(Guid departmentHeadId, Guid doctorId, Guid departmentId, DateTime assignedAt)
    {
        DepartmentHeadId = departmentHeadId;
        DoctorId = doctorId;
        DepartmentId = departmentId;
        AssignedAt = assignedAt;
    }

    protected DepartmentHead() {}
}