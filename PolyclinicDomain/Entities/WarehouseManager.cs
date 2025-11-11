using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Jefe de Almacén (Warehouse Manager).
/// Gestiona un almacén específico.
/// </summary>
public class WarehouseManager : Employee
{
    public Guid? ManagedWarehouseId { get; private set; }
    public Warehouse? ManagedWarehouse { get; private set; }

    public WarehouseManager(Guid id, string name, string employmentStatus, string identification, Guid? managedWarehouseId = null)
        : base(id, name, employmentStatus, identification)
    {
        ManagedWarehouseId = managedWarehouseId;
    }

    // Constructor sin parámetros para EF Core
    private WarehouseManager() { }

    public override string GetPrimaryRole() => ApplicationRoles.WarehouseManager;

    public void AssignWarehouse(Guid warehouseId)
    {
        ManagedWarehouseId = warehouseId;
    }

    public void RemoveWarehouse()
    {
        ManagedWarehouseId = null;
    }
}