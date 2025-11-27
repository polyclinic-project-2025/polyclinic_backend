using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ConsultationReferral
{
    public Guid ConsultationReferralId { get; private set; }
    
    [Required]
    [MaxLength(1000)]
    public string Diagnosis { get; private set; } // Diagnóstico_Rem
    
    public Guid DepartmentHeadId { get; private set; } // ID_J - Jefe que aprueba
    public DepartmentHead? DepartmentHead { get; private set; }

    public Guid DoctorId { get; private set; } // ID_Doc
    public Doctor? Doctor { get; private set; }

    public Guid ReferralId { get; private set; } 
    public Referral? Referral { get; private set; }

    [Required]
    public DateTime DateTimeCRem { get; private set; } // DateTime_CRem

    public ICollection<MedicationReferral> MedicationReferrals { get; private set; } = new List<MedicationReferral>();

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
  
    private ConsultationReferral() { }
}
