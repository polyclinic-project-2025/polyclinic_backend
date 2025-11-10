namespace PolyclinicDomain.Entities;

public class EmergencyRoomCare
{
    public Guid Id { get; private set; }

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = null!;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public Guid? EmergencyRoomId { get; private set; }
    public EmergencyRoom? EmergencyRoom { get; private set; }

    public DateTime CareDate { get; private set; }
    public DateOnly GuardDate { get; private set; }

    public string Diagnosis { get; private set; } = null!;

    public ICollection<MedicationEmergency> MedEmergency { get; private set; } = new List<MedicationEmergency>();

    private EmergencyRoomCare() { }

    public EmergencyRoomCare(Guid id, Guid doctorId, Guid patientId, DateTime careDate, DateOnly guardDate, string diagnosis)
    {
        Id = id;
        DoctorId = doctorId;
        PatientId = patientId;
        CareDate = careDate;
        GuardDate = guardDate;
        Diagnosis = diagnosis;
    }
}
