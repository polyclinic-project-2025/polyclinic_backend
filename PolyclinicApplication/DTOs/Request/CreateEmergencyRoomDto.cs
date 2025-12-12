using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class CreateEmergencyRoomDto
    {
        public Guid DoctorId { get; set; }
        public DateOnly GuardDate { get; set; }
    }
}