namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa una solicitud de almacén realizada por un departamento.
/// Debe ser aprobada por el jefe de departamento solicitante.
/// </summary>
public class WarehouseRequest
{
    public Guid DepartmentId { get; private set; }
    public Guid BossId { get; private set; } // Jefe que aprueba la solicitud
    public Guid WarehouseId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public string? Status { get; private set; }

    // Navigation properties
    public Department? Department { get; private set; }
    public DepartmentHead? Boss { get; private set; }
    public Warehouse? Warehouse { get; private set; }

    public WarehouseRequest(Guid bossId, Guid warehouseId, Guid departmentId, DateTime requestDate, string? status)
    {
        BossId = bossId;
        WarehouseId = warehouseId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        Status = status;
    }

    // Constructor sin parámetros para EF Core
    private WarehouseRequest() { }
}