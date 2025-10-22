namespace PolyclinicDomain.Entities;
public class MedicationEmergency{
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateTime CareDate { get; private set; }
    public DateOnly GuardDate { get; private set; }
    public Guid IdMed { get; private set; }
    public int? Quantity {get; private set;}
    public EmergencyRoomCare Emergency {get;private set;}
    public Medication Medication {get; private set;}
    public MedicationEmergency(Guid doctorId, Guid patientId, DateTime careDate,DateOnly guardDate,Guid idMed,int? quantity){
        DoctorId = doctorId;
        PatientId = patientId;
        CareDate = careDate;
        GuardDate = guardDate;
        IdMed = idMed;
        Quantity = quantity;
    }
}   