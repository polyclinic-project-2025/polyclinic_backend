using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class UpdateMedicationEmergencyDto
    {
        public int? Quantity { get; set; }
        public Guid? EmergencyRoomCareId { get; set; }
        public Guid? MedicationId { get; set; }
    }
}