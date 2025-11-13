
namespace PolyclinicDomain.Entities;

public class Warehouse
{
    public Guid Id { get; private set; }
    public string? Name { get; private set; }

    // Relación 1:1 con WarehouseManager
    public WarehouseManager? Manager { get; private set; }
    public Guid? ManagerId { get; private set; }

    public Warehouse(Guid id, string name, Guid? managerId = null)
    {
        Id = id;
        Name = name;
        ManagerId = managerId;
    }

    // Constructor sin parámetros para EF Core
    private Warehouse() { }

    public void AssignManager(Guid managerId)
    {
        ManagerId = managerId;
    }

    public void RemoveManager()
    {
        ManagerId = null;
    }
}