using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class DepartmentHead
{
    public Guid DepartmentHeadId { get; private set; }
    
    public Guid DoctorId { get; private set; }
    public Doctor? Doctor { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    [Required]
    public DateTime AssignedAt { get; private set; }
    
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>(); 
    public ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>(); 
    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>(); 

    public DepartmentHead(Guid departmentHeadId, Guid doctorId, Guid departmentId, DateTime assignedAt)
    {
        DepartmentHeadId = departmentHeadId;
        DoctorId = doctorId;
        DepartmentId = departmentId;
        AssignedAt = assignedAt;
    }

    private DepartmentHead() {}
}