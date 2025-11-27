using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class EmergencyRoom
{
    public Guid EmergencyRoomId { get; private set; }

    public Guid DoctorId { get; private set; }
    public Doctor? Doctor { get; private set; }

    [Required]
    public DateOnly GuardDate { get; private set; }
    
    public ICollection<EmergencyRoomCare> EmergencyRoomCares { get; private set; } = new List<EmergencyRoomCare>();
    
    public EmergencyRoom(Guid emergencyRoomId, Guid doctorId, DateOnly guardDate)
    {
        EmergencyRoomId = emergencyRoomId;
        DoctorId = doctorId;
        GuardDate = guardDate;
    }

    private EmergencyRoom() { }
}