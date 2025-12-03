using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class UpdateMedicationDto
{
    public string Format { get; set; } = string.Empty;
    public string CommercialName { get; set; } = string.Empty;
    public string CommercialCompany { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public string ScientificName { get; set; } = string.Empty;
    public int QuantityWarehouse { get; set; }
    public int QuantityNurse { get; set; }
}