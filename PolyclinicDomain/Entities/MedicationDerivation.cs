namespace PolyclinicDomain.Entities;
public class MedicationDerivation
{
    public Guid DepartmentToId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateTime DateTimeDer { get; private set; }
    public DateTime DateTimeCDer { get; private set; }
    public Guid? DepartmentFromId { get; private set; }
    public Guid? DoctorId { get; private set; }
    public Guid IdMed { get; private set; }
    public int? Quantity { get; private set; }
    public Medication Medication {get;private set;}
    public ConsultationDerivation Consulta {get; private set;}
    public MedicationDerivation(Guid departmentToId, Guid? departmentFromId,Guid patientId,DateTime dateTimeDer, DateTime dateTimeCDer,Guid? doctorId,Guid idMed, int? quantity){
        DepartmentToId = departmentToId;
        DepartmentFromId = departmentFromId;
        PatientId = patientId;
        DateTimeDer = dateTimeDer;
        DateTimeCDer = dateTimeCDer;
        IdMed = idMed;
        Quantity = quantity;
        DoctorId =doctorId;
    }
}