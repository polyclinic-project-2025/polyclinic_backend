using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class CreateMedicationEmergencyDto
    {
        public int Quantity { get; set; }
        public Guid EmergencyRoomCareId { get; set; }
        public Guid MedicationId { get; set; }
    }
}