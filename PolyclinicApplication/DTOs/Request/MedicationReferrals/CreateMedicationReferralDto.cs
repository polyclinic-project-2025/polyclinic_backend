namespace PolyclinicApplication.DTOs.Request.MedicationReferrals;

public class CreateMedicationReferralDto
{
    public int Quantity { get; set; }
    public Guid ConsultationReferralId { get; set; }
    public Guid MedicationId { get; set; }
}
