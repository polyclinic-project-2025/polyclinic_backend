using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class MedicationDerivation
{
    public Guid MedicationDerivationId { get; private set; }
    
    [Required]
    [Range(0, 1000000)]
    public int Quantity { get; private set; }

    public Guid ConsultationDerivationId { get; private set; }
    public virtual ConsultationDerivation? ConsultationDerivation {get; private set;}

    public Guid MedicationId { get; private set; }
    public virtual Medication? Medication { get; private set; }

    public MedicationDerivation(
        Guid medicationDerivationId,
        int quantity,
        Guid consultationDerivationId,
        Guid medicationId)
    {
        MedicationDerivationId = medicationDerivationId;
        Quantity = quantity;
        ConsultationDerivationId = consultationDerivationId;
        MedicationId = medicationId;
    }

    protected MedicationDerivation(){}

    // Update methods
    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;

    }

    public void UpdateConsultationDerivationId(Guid consultationDerivationId)
    {
        ConsultationDerivationId = consultationDerivationId;
    }

    public void UpdateMedicationId(Guid medicationId)
    {
        MedicationId = medicationId;
    }
}