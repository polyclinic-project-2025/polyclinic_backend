namespace PolyclinicApplication.DTOs.Request.Referral{

public class CreateReferralDto
{
    public string PuestoExterno { get; set; }
    public DateTime DateTimeRem {get; set; }
    public Guid PatientId { get; set; }
    public Guid DepartmentToId { get; set; }
}

}