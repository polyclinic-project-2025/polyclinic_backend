using System;
namespace PolyclinicApplication.DTOs.Response.Derivations{

    public class DerivationDto
{
    public Guid DerivationId { get; set; }
    public DateTime DateTimeDer { get; set; }

    // IDs planos
    public Guid DepartmentFromId { get; set; }
    public Guid DepartmentToId { get; set; }
    public Guid PatientId { get; set; }

    // Información útil del frontend (campos navegados)
    public string? DepartmentFromName { get; set; }
    public string? DepartmentToName { get; set; }
    public string? PatientName { get; set; }
    public string? PatientIdentification {get;set;}
}
}

