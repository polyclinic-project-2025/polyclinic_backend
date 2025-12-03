using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class CreateConsultationDerivationDto
{
    public string Diagnosis { get; set; } = string.Empty;
    public Guid DerivationId { get; set; }
    public DateTime DateTimeCDer { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentHeadId { get; set; }
    
}