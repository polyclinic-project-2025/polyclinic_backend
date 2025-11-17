namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa una solicitud de almacén realizada por un departamento.
/// Debe ser aprobada por el jefe de departamento solicitante.
/// </summary>
public class WarehouseRequest
{
    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Department Department { get; private set; } = null!;

    public Guid BossId { get; private set; }
    public DepartmentHead Boss { get; private set; } = null!;

    public Guid WarehouseId { get; private set; }
    public Warehouse Warehouse { get; private set; } = null!;

    public DateTime RequestDate { get; private set; }

    public string Status { get; private set; }

    private WarehouseRequest() { }

    public WarehouseRequest(Guid id, Guid bossId, Guid warehouseId, Guid departmentId, DateTime requestDate, string status)
    {
        Id = id;
        BossId = bossId;
        WarehouseId = warehouseId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        Status = status;
    }

}