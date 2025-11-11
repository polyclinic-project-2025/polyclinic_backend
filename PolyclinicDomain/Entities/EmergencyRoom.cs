namespace PolyclinicDomain.Entities;

public class EmergencyRoom
{
    public Guid Id { get; private set; }

    public Doctor? Doctor { get; private set; }
    public Guid DoctorId { get; private set; }

    public DateOnly GuardDate { get; private set; }

    public ICollection<EmergencyRoomCare> Cares { get; private set; } = new List<EmergencyRoomCare>();
    
    public EmergencyRoom(Guid doctorId, DateOnly guardDate)
    {
        DoctorId = doctorId;
        GuardDate = guardDate;
    }

    // constructor para EF
    private EmergencyRoom() { }
}