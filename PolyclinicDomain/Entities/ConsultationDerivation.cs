using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ConsultationDerivation
{
    public Guid ConsultationDerivationId { get; private set; }
    
    [Required]
    [MaxLength(1000)]
    public string Diagnosis { get; private set; } // Diagnostico_Der

    public Guid DepartmentHeadId { get; private set; } // ID_J 
    public DepartmentHead? DepartmentHead { get; private set; }

    public Guid DoctorId { get; private set; } // ID_Doc
    public Doctor? Doctor { get; private set; }

    public Guid DerivationId { get; private set; } 
    public Derivation? Derivation { get; private set; }
    
    [Required]
    public DateTime DateTimeCDer { get; private set; } // DateTime_CDer

    public ICollection<MedicationDerivation> MedicationDerivations { get; private set; } = new List<MedicationDerivation>();

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
}
