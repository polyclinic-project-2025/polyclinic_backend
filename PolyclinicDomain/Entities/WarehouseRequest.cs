using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class WarehouseRequest
{
    public Guid WarehouseRequestId { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Status { get; private set; }
    
    [Required]
    public DateTime RequestDate { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    public Guid WarehouseManagerId { get; private set; }
    public WarehouseManager? WarehouseManager { get; private set; }

    public ICollection<MedicationRequest> MedicationRequests { get; private set; } = new List<MedicationRequest>();

    public WarehouseRequest(
        Guid warehouseRequestId,
        string status,
        DateTime requestDate,
        Guid departmentId,
        Guid warehouseManagerId)
    {
        WarehouseRequestId = warehouseRequestId;
        Status = status;
        RequestDate = requestDate;
        DepartmentId = departmentId;
        WarehouseManagerId = warehouseManagerId;
    }

    private WarehouseRequest() { }
}