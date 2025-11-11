namespace PolyclinicDomain.Entities;

public class EmergencyRoomCare
{
    public Guid Id { get; private set; }

    public Guid EmergencyRoomId { get; private set; }
    public EmergencyRoom EmergencyRoom { get; private set; } = null!;

    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;

    public DateTime CareDate { get; private set; }

    public string Diagnosis { get; private set; } = null!;

    public ICollection<MedicationEmergency> MedEmergency { get; private set; } = new List<MedicationEmergency>();

    private EmergencyRoomCare() { }

    public EmergencyRoomCare(Guid id, Guid emergencyRoomId, Guid patientId, DateTime careDate, string diagnosis)
    {
        Id = id;
        EmergencyRoomId = emergencyRoomId;
        PatientId = patientId;
        CareDate = careDate;
        Diagnosis = diagnosis;
    }
}
