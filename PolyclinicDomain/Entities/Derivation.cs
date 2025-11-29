using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Derivation
{
    public Guid DerivationId { get; private set; }
    
    public Guid DepartmentFromId { get; private set; } // ID_Dpt1
    public Department? DepartmentFrom { get; private set; }

    [Required]
    public DateTime DateTimeDer { get; private set; } // DateTime_Der

    public Guid PatientId { get; private set; } // ID_Pac
    public Patient? Patient { get; private set; }
    
    public Guid DepartmentToId { get; private set; } // ID_Dpt2
    public Department? DepartmentTo { get; private set; }

    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>();
    
    public Derivation(Guid derivationId,
        Guid departmentFromId,
        DateTime dateTimeDer,
        Guid patientId,
        Guid departmentToId)
    {
        DerivationId = derivationId;
        DepartmentFromId = departmentFromId;
        DateTimeDer = dateTimeDer;
        PatientId = patientId;
        DepartmentToId = departmentToId;
    }
}
