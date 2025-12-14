using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class UpdateEmergencyRoomDto
    {
        public Guid? DoctorId { get; set; }
        public DateOnly? GuardDate { get; set; }
    }
}