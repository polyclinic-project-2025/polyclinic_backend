using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class WarehouseManager : Employee
{
    public Guid WarehouseId { get; private set; }
    public Warehouse? Warehouse { get; private set; }

    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public string? UserId { get; set; }

    public WarehouseManager(Guid id, string identification, string name, string employmentStatus, Guid warehouseId)
        : base(id, identification, name, employmentStatus)
    {
        WarehouseId = warehouseId;
    }

    private WarehouseManager() { }

    public override string GetPrimaryRole() => ApplicationRoles.WarehouseManager;

    public void AssignWarehouse(Guid warehouseId)
    {
        WarehouseId = warehouseId;
    }
}