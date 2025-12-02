
namespace PolyclinicApplication.DTOs.Request.Consultations;
using System.ComponentModel.DataAnnotations;

public record CreateConsultationReferralDto
{
    [Required(ErrorMessage = "Es requerido el paciente remitido")]
    public Guid ReferralId { get; set; } = Guid.Empty;
    [Required(ErrorMessage = "Es requerido el doctor tratante")]
    public Guid DoctorId { get; set; } = Guid.Empty;
    
    [Required(ErrorMessage ="La fecha es requerida")]
    public DateTime DateTimeCRem { get; set;  }

    [Required(ErrorMessage = "El jefe de departamento es requerido")]
    public Guid DepartmentHeadId { get; set; } = Guid.Empty;
    [Required(ErrorMessage ="Debe proporcionar un diagnóstico")]
    public string Diagnosis { get; set; } = string.Empty;
}