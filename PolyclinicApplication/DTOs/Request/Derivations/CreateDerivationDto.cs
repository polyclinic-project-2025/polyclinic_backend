namespace PolyclinicApplication.DTOs.Request.Derivations{

public class CreateDerivationDto
{
    public Guid DepartmentFromId { get; set; }
    public DateTime DateTimeDer { get; set; }
    public Guid PatientId { get; set; }
    public Guid DepartmentToId { get; set; }
}

}

