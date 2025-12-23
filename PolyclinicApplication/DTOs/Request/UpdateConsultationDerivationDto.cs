using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class UpdateConsultationDerivationDto
{
    public string Diagnosis { get; set; } = string.Empty;
    public DateTime DateTimeCDer { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentHeadId { get; set; }
    
}