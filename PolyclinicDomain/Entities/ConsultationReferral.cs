using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ConsultationReferral
{
    public Guid ConsultationReferralId { get; private set; }
    
    [Required]
    [MaxLength(1000)]
    public string Diagnosis { get; set; } // Diagnóstico_Rem
    
    public Guid DepartmentHeadId { get; set; } // ID_J - Jefe que aprueba
    public virtual DepartmentHead? DepartmentHead { get; set; }

    public Guid DoctorId { get; set; } // ID_Doc
    public virtual Doctor? Doctor { get; set; }

    public Guid ReferralId { get; set; } 
    public virtual Referral? Referral { get; set; }

    [Required]
    public DateTime DateTimeCRem { get; set; } // DateTime_CRem

    public virtual ICollection<MedicationReferral> MedicationReferrals { get; private set; } = new List<MedicationReferral>();

    public ConsultationReferral(
        Guid consultationReferralId,
        string diagnosis,
        Guid departmentHeadId,
        Guid doctorId,
        Guid referralId,
        DateTime dateTimeCRem)
    {
        ConsultationReferralId = consultationReferralId;
        Diagnosis = diagnosis;
        DepartmentHeadId = departmentHeadId;
        DoctorId = doctorId;
        ReferralId = referralId;
        DateTimeCRem = dateTimeCRem;
    }
  
    protected ConsultationReferral() { }
}
