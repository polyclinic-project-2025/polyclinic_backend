using System;

namespace PolyclinicApplication.DTOs.Request.MedicationDerivation;

public class CreateMedicationDerivationDto
{
    public int Quantity { get; set; }
    public Guid ConsultationDerivationId { get; set; }
    public Guid MedicationId { get; set; }
}