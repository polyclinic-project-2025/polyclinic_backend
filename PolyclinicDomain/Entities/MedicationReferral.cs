namespace PolyclinicDomain.Entities;
public class MedicationReferral
{
    public Guid? DoctorId { get; private set; }
    public Guid ExternalMedicalPostId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateTime DateTimeRem { get; private set; }
    public DateTime DateTimeCRem { get; private set; }
    public Guid? DepartmentToId { get; private set; }
    public string? Diagnosis { get; private set; }
    public Guid IdMed { get; private set; }
    public int? Quantity {get;private set;}
    public Medication Medication {get;private set;}
    public ConsultationReferral Consulta {get; private set;}
    public MedicationReferral(Guid? doctorId, Guid externalMedicalPostId, Guid patientId,DateTime dateTimeRem, DateTime dateTimeCRem,Guid? departmentToId, string? diagnosis, Guid idMed,int? quantity){
        DoctorId = doctorId;
        ExternalMedicalPostId = externalMedicalPostId;
        PatientId = patientId;
        DateTimeRem = dateTimeRem;
        DateTimeCRem = dateTimeCRem;
        DepartmentToId = departmentToId;
        Diagnosis = diagnosis;
        IdMed = idMed;
        Quantity = quantity;
    }
}