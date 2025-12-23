using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class WarehouseManager : Employee
{
    [Required]
    public DateTime AssignedAt { get; private set; }

    public virtual ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public WarehouseManager(Guid id, string identification, string name, string employmentStatus, DateTime assignedAt)
        : base(id, identification, name, employmentStatus) 
    {
        AssignedAt = assignedAt;
    }

    protected WarehouseManager() { }

    public override string GetPrimaryRole() => ApplicationRoles.WarehouseManager;
    
    public Guid GetEmployeeId()
    {
        return EmployeeId;
    }
}