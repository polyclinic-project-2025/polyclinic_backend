using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ConsultationDerivation
{
    public Guid ConsultationDerivationId { get; private set; }
    
    [Required]
    [MaxLength(1000)]
    public string Diagnosis { get; private set; } // Diagnostico_Der

    public Guid DepartmentHeadId { get; private set; } // ID_J 
    public virtual DepartmentHead? DepartmentHead { get; private set; }

    public Guid DoctorId { get; private set; } // ID_Doc
    public virtual Doctor? Doctor { get; private set; }

    public Guid DerivationId { get; private set; } 
    public virtual Derivation? Derivation { get; private set; }
    
    [Required]
    public DateTime DateTimeCDer { get; private set; } // DateTime_CDer

    public  virtual ICollection<MedicationDerivation> MedicationDerivations { get; private set; } = new List<MedicationDerivation>();

    public ConsultationDerivation(
        Guid consultationDerivationId,
        string diagnosis,
        Guid derivationId,
        DateTime dateTimeCDer,
        Guid doctorId,
        Guid departmentHeadId)
    {
        ConsultationDerivationId = consultationDerivationId;
        Diagnosis = diagnosis;
        DerivationId = derivationId;
        DateTimeCDer = dateTimeCDer;
        DoctorId = doctorId;
        DepartmentHeadId = departmentHeadId;
    }

    private ConsultationDerivation() { }

    public void UpdateDiagnosis(string diagnosis)
    {
        Diagnosis = diagnosis;
    }

    public void UpdateDateTimeCDer(DateTime dateTime)
    {
        DateTimeCDer = dateTime;
    }

    public void UpdateDoctorId(Guid doctorId)
    {
        DoctorId = doctorId;
    }

    public void UpdateDepartmentHeadId(Guid deptHeadId)
    {
        DepartmentHeadId = deptHeadId;
    }
}
