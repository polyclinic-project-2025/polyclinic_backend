using System;

namespace PolyclinicApplication.DTOs.Response
{
    public class EmergencyRoomDto
    {
        public Guid EmergencyRoomId { get; set; }
        public DateOnly GuardDate { get; set; }
        
        // Datos del Doctor
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorIdentification { get; set; }
    }
}