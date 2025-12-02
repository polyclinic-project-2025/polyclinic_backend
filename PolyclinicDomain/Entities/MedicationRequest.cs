using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class MedicationRequest
{
    public Guid MedicationRequestId { get; private set; }
    
    [Required]
    [Range(0, 1000000)]
    public int Quantity { get; private set; }
    
    public Guid WarehouseRequestId { get; private set; }
    public virtual WarehouseRequest? WarehouseRequest { get; private set; }

    public Guid MedicationId { get; private set; }
    public virtual Medication? Medication { get; private set; }

    public MedicationRequest(
        Guid medicationRequestId,
        int quantity,
        Guid warehouseRequestId,
        Guid medicationId)
    {
        MedicationRequestId = medicationRequestId;
        Quantity = quantity;
        WarehouseRequestId = warehouseRequestId;
        MedicationId = medicationId;
    }

    protected MedicationRequest() { }
}
