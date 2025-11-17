namespace PolyclinicDomain.Entities;

public class Referral
{
    public Guid PatientId { get; private set; } // ID_Pac
    public Guid DepartmentToId { get; private set; } // ID_Dpt2
    public DateTime DateTimeRem { get; private set; } // DateTime_Rem
    public Guid ExternalMedicalPostId { get; private set; } // ID_Ext
    public Patient? Patient { get; private set; }
    public Department? DepartmentTo { get; private set; } 
    public ExternalMedicalPost? ExternalMedicalPost { get; private set; }

    public Referral(Guid patientId, Guid departmentToId, DateTime dateTimeRem, Guid externalMedicalPostId)
    {
        PatientId = patientId;
        DepartmentToId = departmentToId;
        DateTimeRem = dateTimeRem;
        ExternalMedicalPostId = externalMedicalPostId;
    }
}
