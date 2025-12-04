using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class WarehouseManager : Employee
{
    public Guid WarehouseId { get; private set; }
    public virtual Warehouse? Warehouse { get; private set; }

    public virtual ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public WarehouseManager(Guid id, string identification, string name, string employmentStatus, Guid warehouseId)
        : base(id, identification, name, employmentStatus)
    {
        WarehouseId = warehouseId;
    }

    protected WarehouseManager() { }

    public override string GetPrimaryRole() => ApplicationRoles.WarehouseManager;

    public void AssignWarehouse(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }
}