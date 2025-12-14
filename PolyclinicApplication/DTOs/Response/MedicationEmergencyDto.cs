using System;

namespace PolyclinicApplication.DTOs.Response
{
    public class MedicationEmergencyDto
    {
        public Guid MedicationEmergencyId { get; set; }
        public int Quantity { get; set; }
        public Guid EmergencyRoomCareId { get; set; }
        public Guid MedicationId { get; set; }
        
        // Nombre del medicamento
        public string CommercialName { get; set; }
    }
}