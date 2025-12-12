using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class MedicationEmergency{
    public Guid MedicationEmergencyId { get; private set; }
    
    [Required]
    [Range(0, 1000000)]
    public int Quantity {get; set;}

    public Guid EmergencyRoomCareId { get; set; }
    public virtual EmergencyRoomCare? EmergencyRoomCare { get; private set; }

    public Guid MedicationId { get; set; }
    public virtual Medication? Medication { get; private set; }
    
    public MedicationEmergency(
        Guid medicationEmergencyId,
        int quantity,
        Guid emergencyRoomCareId,
        Guid medicationId)
    {
        MedicationEmergencyId = medicationEmergencyId;
        Quantity = quantity;
        EmergencyRoomCareId = emergencyRoomCareId;
        MedicationId = medicationId;
    }

    protected MedicationEmergency(){}
}