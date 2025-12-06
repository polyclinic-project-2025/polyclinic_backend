using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class WarehouseManager : Employee
{
    public virtual ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public WarehouseManager(Guid id, string identification, string name, string employmentStatus)
        : base(id, identification, name, employmentStatus) {}

    protected WarehouseManager() { }

    public override string GetPrimaryRole() => ApplicationRoles.WarehouseManager;
}