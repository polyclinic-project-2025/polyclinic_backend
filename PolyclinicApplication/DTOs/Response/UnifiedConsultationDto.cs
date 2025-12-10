using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;
public class UnifiedConsultationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } // "Derivation" o "Referral"
    public DateTime ConsultationDate { get; set; }
    public string Diagnosis { get; set; }
    public Guid PatientId { get; set; }
    public string PatientFullName { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorFullName { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    public List<MedicationInfoDto> Medications { get; set; } = new List<MedicationInfoDto>();
}