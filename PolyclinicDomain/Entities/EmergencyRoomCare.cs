namespace PolyclinicDomain.Entities;

public class EmergencyRoomCare
{
    public Doctor? Doctor { get; private set; }
    public Guid DoctorId { get; private set; }

    public Patient? Patient { get; private set; } 
    public Guid PatientId { get; private set; }

    public DateTime CareDate { get; private set; }
    public DateOnly GuardDate { get; private set; }

    public string Diagnosis { get; private set; }

    public EmergencyRoomCare(Guid doctorId, Guid patientId, DateTime careDate, DateOnly guardDate, string diagnosis)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        CareDate = careDate;
        GuardDate = guardDate;
        Diagnosis = diagnosis;
    }
}
