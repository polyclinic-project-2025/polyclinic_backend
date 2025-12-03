using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class ConsultationDerivationDto
{
    public Guid ConsultationDerivationId { get; set; }

    public string Diagnosis { get; set; } = null!;

    public DateTime DateTimeCDer { get; set; }

    public Guid DerivationId { get; set; }

    public Guid DoctorId { get; set; }
    public string? DoctorName { get; set; }

    public Guid DepartmentHeadId { get; set; }
    public string? DepartmentHeadName { get; set; }

    // Info del paciente obtenida a través de Derivation
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }

    // DepartmentTo (also from Derivation)
    public Guid DepartmentToId { get; set; }
    public string DepartmentToName { get; set; } = default!;
}