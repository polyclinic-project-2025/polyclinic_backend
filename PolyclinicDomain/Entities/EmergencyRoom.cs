namespace PolyclinicDomain.Entities;

public class EmergencyRoom
{
    public Doctor? Doctor { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateOnly GuardDate { get; private set; }

    public EmergencyRoom(Guid doctorId, DateOnly guardDate)
    {
        DoctorId = doctorId;
        GuardDate = guardDate;
    }
}