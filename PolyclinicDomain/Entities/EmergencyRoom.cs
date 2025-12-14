using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class EmergencyRoom
{
    public Guid EmergencyRoomId { get; private set; }

    public Guid DoctorId { get; set; }
    public virtual Doctor? Doctor { get; private set; }

    [Required]
    public DateOnly GuardDate { get; set; }
    
    public virtual ICollection<EmergencyRoomCare> EmergencyRoomCares { get; private set; } = new List<EmergencyRoomCare>();
    
    public EmergencyRoom(Guid emergencyRoomId, Guid doctorId, DateOnly guardDate)
    {
        EmergencyRoomId = emergencyRoomId;
        DoctorId = doctorId;
        GuardDate = guardDate;
    }

    protected EmergencyRoom() { }
}