namespace PolyclinicApplication.DTOs.Response.Consultations;

public record ConsultationReferralResponse
{
    public Guid ConsultationReferralId { get; set; } = Guid.Empty;
    public Guid ReferralId { get; set; } = Guid.Empty;
    public Guid DoctorId { get; set; } = Guid.Empty;
    public DateTime DateTimeCRem { get; set; }
    public Guid DepartmentHeadId { get; set; } = Guid.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DoctorFullName { get; set; } = string.Empty;
    public string PatientFullName { get; set; } = string.Empty;
}