using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class MedicationReferral
{
    public Guid MedicationReferralId { get; private set; }
    
    [Required]
    [Range(0, 1000000)]
    public int Quantity {get;private set;}
    
    public Guid ConsultationReferralId { get; private set; }
    public virtual ConsultationReferral? ConsultationReferral { get; private set; }

    public Guid MedicationId { get; private set; }
    public virtual Medication? Medication { get; private set; }
    
    public MedicationReferral(
        Guid medicationReferralId,
        int quantity,
        Guid consultationReferralId,
        Guid medicationId)
    {
        MedicationReferralId = medicationReferralId;
        Quantity = quantity;
        ConsultationReferralId = consultationReferralId;
        MedicationId = medicationId;
    }

    protected MedicationReferral(){}
}