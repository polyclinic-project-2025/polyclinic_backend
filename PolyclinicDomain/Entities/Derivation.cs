namespace PolyclinicDomain.Entities;

public class Derivation
{
    public Guid DepartmentFromId { get; private set; } // ID_Dpt1
    public Guid PatientId { get; private set; } // ID_Pac
    public Guid DepartmentToId { get; private set; } // ID_Dpt2
    public DateTime DateTimeDer { get; private set; } // DateTime_Der
    public Patient? Patient { get; private set; } 
    public Department? DepartmentFrom { get; private set; }
    public Department? DepartmentTo { get; private set; }
    public Derivation(Guid departmentFromId, Guid patientId, Guid departmentToId, DateTime dateTimeDer)
    {
        DepartmentFromId = departmentFromId;
        PatientId = patientId;
        DepartmentToId = departmentToId; 
        DateTimeDer = dateTimeDer;
    }
}
