namespace PolyclinicDomain.Entities;

public class WarehouseRequest
{
    public Department? Department { get; private set; }
    public Guid DepartmentId { get; private set; }

    public Boss? Boss { get; private set; }
    public Guid BossId { get; private set; }

    public Warehouse? Warehouse { get; private set; }
    public Guid WarehouseId { get; private set; }

    public DateTime RequestDate { get; private set; }
    public string? Status { get; private set; }

    public WarehouseRequest(Guid bossId, Guid warehouseId, Guid departmentId, DateTime requestDate, string? status)
    {
        BossId = bossId;
        WarehouseId = warehouseId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        Status = status;
    }
}
