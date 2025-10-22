namespace PolyclinicDomain.Entities;

public class ConsultationReferral
{
    public Guid BossId { get; private set; } // ID_J
    public Guid ExternalMedicalPostId { get; private set; } // ID_Ext
    public Guid PatientId { get; private set; } // ID_Pac
    public DateTime DateTimeRem { get; private set; } // DateTime_Rem
    public DateTime DateTimeCRem { get; private set; } // DateTime_CRem
    public Guid? DepartmentToId { get; private set; } // ID_Dpt2
    public Guid? DoctorId { get; private set; } // ID_Doc
    public string? Diagnosis { get; private set; } // Diagnóstico_Rem
    public Patient? Patient { get; private set; }
    public Boss? Boss { get; private set; }
    public Doctor? Doctor { get; private set; }
    public ExternalMedicalPost? ExternalMedicalPost { get; private set; }
    public Department? DepartmentTo { get; private set; }

    public ConsultationReferral(Guid bossId, Guid externalMedicalPostId, Guid patientId, DateTime dateTimeRem, DateTime dateTimeCRem, Guid? departmentToId = null, Guid? doctorId = null, string? diagnosis = null)
    {
        BossId = bossId;
        ExternalMedicalPostId = externalMedicalPostId;
        PatientId = patientId;
        DateTimeRem = dateTimeRem;
        DateTimeCRem = dateTimeCRem;
        DepartmentToId = departmentToId;
        DoctorId = doctorId;
        Diagnosis = diagnosis;
    }
}
