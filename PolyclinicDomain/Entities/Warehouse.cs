using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Warehouse
{
    public Guid WarehouseId { get; private set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; }

    public WarehouseManager? WarehouseManager { get; private set; }

    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public Warehouse(Guid warehouseId, string name)
    {
        WarehouseId = warehouseId;
        Name = name;
    }

    private Warehouse() { }
}