namespace PolyclinicApplication.DTOs.Request.MedicationDerivation;

public class UpdateMedicationDerivationDto
{
    public int? Quantity { get; set; }
    public Guid? ConsultationDerivationId { get; set; }
    public Guid? MedicationId { get; set; }
}