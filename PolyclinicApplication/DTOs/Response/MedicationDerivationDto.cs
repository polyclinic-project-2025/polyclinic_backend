using System;

namespace PolyclinicApplication.DTOs.Response
{
    public class MedicationDerivationDto
    {
        public Guid MedicationDerivationId { get; set; }
        public int Quantity { get; set; }
        public Guid ConsultationDerivationId { get; set; }
        public Guid MedicationId { get; set; }
    }
}