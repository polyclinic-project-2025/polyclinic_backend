namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa una consulta de referencia a un puesto médico externo.
/// Requiere aprobación del jefe de departamento.
/// </summary>
public class ConsultationReferral
{
    public Guid ApprovedByHeadId { get; private set; } // ID_J - Jefe que aprueba
    public Guid ExternalMedicalPostId { get; private set; } // ID_Ext
    public Guid PatientId { get; private set; } // ID_Pac
    public DateTime DateTimeRem { get; private set; } // DateTime_Rem
    public DateTime DateTimeCRem { get; private set; } // DateTime_CRem
    public Guid? DepartmentToId { get; private set; } // ID_Dpt2
    public Guid? DoctorId { get; private set; } // ID_Doc
    public string? Diagnosis { get; private set; } // Diagnóstico_Rem

    // Navigation properties
    public Patient? Patient { get; private set; }
    public DepartmentHead? ApprovedByHead { get; private set; }
    public Doctor? Doctor { get; private set; }
    public ExternalMedicalPost? ExternalMedicalPost { get; private set; }
    public Department? DepartmentTo { get; private set; }
    public ICollection<MedicationReferral> MedRem { get; set; } = new List<MedicationReferral>();

    public ConsultationReferral(Guid approvedByHeadId, Guid externalMedicalPostId, Guid patientId, DateTime dateTimeRem, DateTime dateTimeCRem, Guid? departmentToId = null, Guid? doctorId = null, string? diagnosis = null)
    {
        ApprovedByHeadId = approvedByHeadId;
        ExternalMedicalPostId = externalMedicalPostId;
        PatientId = patientId;
        DateTimeRem = dateTimeRem;
        DateTimeCRem = dateTimeCRem;
        DepartmentToId = departmentToId;
        DoctorId = doctorId;
        Diagnosis = diagnosis;
    }

    // Constructor sin parámetros para EF Core
    private ConsultationReferral() { }
}
