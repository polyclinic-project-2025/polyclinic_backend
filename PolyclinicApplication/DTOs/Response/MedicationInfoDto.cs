using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class MedicationInfoDto
{
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}