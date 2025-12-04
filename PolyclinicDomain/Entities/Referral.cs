using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Referral
{
    public Guid ReferralId {get; private set; }
    
    public Guid PatientId { get; private set; } // ID_Pac
    public virtual Patient? Patient { get; private set; }

    [Required]
    public DateTime DateTimeRem { get; private set; } // DateTime_Rem

    public Guid ExternalMedicalPostId { get; private set; } // ID_Ext
    public virtual ExternalMedicalPost? ExternalMedicalPost { get; private set; }

    public Guid DepartmentToId { get; private set; } // ID_Dpt2
    public virtual Department? DepartmentTo { get; private set; } 

    public virtual ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>();

    public Referral(
        Guid referralId,
        Guid patientId,
        DateTime dateTimeRem,
        Guid externalMedicalPostId,
        Guid departmentToId)
    {
        ReferralId = referralId;
        PatientId = patientId;
        DateTimeRem = dateTimeRem;
        ExternalMedicalPostId = externalMedicalPostId;
        DepartmentToId = departmentToId;
    }

    protected Referral(){}
}
