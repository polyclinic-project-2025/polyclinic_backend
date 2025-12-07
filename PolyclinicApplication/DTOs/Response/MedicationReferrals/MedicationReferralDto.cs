namespace PolyclinicApplication.DTOs.Response.MedicationReferrals;

public class MedicationReferralDto
{
    public Guid MedicationReferralId { get; set; }
    public int Quantity { get; set; }
    public Guid ConsultationReferralId { get; set; }
    public Guid MedicationId { get; set; }
}
