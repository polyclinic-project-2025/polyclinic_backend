namespace PolyclinicApplication.DTOs.Request.Consultations;

public record UpdateConsultationReferralDto
{
    public Guid? ReferralId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? DateTimeCRem { get; set; }
    public Guid? DepartmentHeadId { get; set; }
    public string? Diagnosis { get; set; }
}
