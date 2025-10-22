namespace PolyclinicDomain.Entities;

public class ConsultationDerivation
{
    public Guid BossId { get; private set; } // ID_J
    public Guid DepartmentToId { get; private set; } // ID_Dpt2
    public Guid PatientId { get; private set; } // ID_Pac
    public DateTime DateTimeDer { get; private set; } // DateTime_Der 
    public DateTime DateTimeCDer { get; private set; } // DateTime_CDer 
    public Guid? DepartmentFromId { get; private set; } // ID_Dpt1
    public Guid? DoctorId { get; private set; } // ID_Doc
    public string? Diagnosis { get; private set; } // Diagnostico_Der
    public Patient? Patient { get; private set; }
    public Boss? Boss { get; private set; } 
    public Doctor? Doctor { get; private set; }
    public Department? DepartmentFrom { get; private set; }
    public Department? DepartmentTo { get; private set; }
    public ICollection<MedicationDerivation> MedDer {get; set;}
    public ConsultationDerivation(Guid bossId, Guid departmentToId, Guid patientId, DateTime dateTimeDer, DateTime dateTimeCDer, Guid? departmentFromId = null, Guid? doctorId = null, string? diagnosis = null)
    {
        BossId = bossId;
        DepartmentToId = departmentToId;
        PatientId = patientId;
        DateTimeDer = dateTimeDer;
        DateTimeCDer = dateTimeCDer;
        DepartmentFromId = departmentFromId;
        DoctorId = doctorId;
        Diagnosis = diagnosis;
    }
}
