using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class UpdateEmergencyRoomCareDto
    {
        public string? Diagnosis { get; set; }
        public Guid? EmergencyRoomId { get; set; }
        public DateTime? CareDate { get; set; }
        public Guid? PatientId { get; set; }
    }
}