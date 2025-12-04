using System;

namespace PolyclinicApplication.DTOs.Response.Referral
{
    public class ReferralDto
    {
    public Guid ReferralId { get; set; }
    public DateTime DateTimeRem { get; set; }

    // IDs planos
    public Guid ExternalMedicalPostId { get; set; }
    public Guid DepartmentToId { get; set; }
    public Guid PatientId { get; set; }

    // Información útil del frontend (campos navegados)
    public string? PuestoExterno { get; set; }
    public string? DepartmentToName { get; set; }
    public string? PatientName { get; set; }
    public string? PatientIdentification {get;set;}
}
    }
