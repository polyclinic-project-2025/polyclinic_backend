using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class CreateEmergencyRoomCareDto
    {
        public string Diagnosis { get; set; } = string.Empty;
        public Guid EmergencyRoomId { get; set; }
        public DateTime CareDate { get; set; }
        public Guid PatientId { get; set; }
    }
}