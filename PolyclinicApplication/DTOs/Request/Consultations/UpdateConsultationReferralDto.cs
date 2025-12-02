
namespace PolyclinicApplication.DTOs.Request.Consultations;

public record UpdateConsultationReferralDto
{
    public Guid? ReferralId { get; set; } = Guid.Empty;

    public Guid? DoctorId { get; set; } = Guid.Empty;

    public DateTime? DateTimeCRem { get; set;  }

    public Guid? DepartmentHeadId { get; set; } = Guid.Empty;

    public string? Diagnosis { get; set; } = string.Empty;
}