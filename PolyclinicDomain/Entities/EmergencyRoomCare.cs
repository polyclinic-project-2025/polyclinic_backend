using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class EmergencyRoomCare
{
    public Guid EmergencyRoomCareId { get; private set; }

    [Required]
    [MaxLength(1000)]
    public string Diagnosis { get; private set; }
    
    public Guid EmergencyRoomId { get; private set; }
    public virtual EmergencyRoom? EmergencyRoom { get; private set; }

    [Required]
    public DateTime CareDate { get; private set; }

    public Guid PatientId { get; private set; }
    public virtual Patient? Patient { get; private set; }
    
    public virtual ICollection<MedicationEmergency> MedicationEmergencies { get; private set; } = new List<MedicationEmergency>();

    protected EmergencyRoomCare() { }

    public EmergencyRoomCare(
        Guid emergencyRoomCareId,
        string diagnosis,
        Guid emergencyRoomId,
        DateTime careDate,
        Guid patientId)
    {
        EmergencyRoomCareId = emergencyRoomCareId;
        Diagnosis = diagnosis;
        EmergencyRoomId = emergencyRoomId;
        CareDate = careDate;
        PatientId = patientId;
    }
}
