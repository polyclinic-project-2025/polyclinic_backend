using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class MedicationResponseDto
{
    public Guid MedicationId { get; set; }
    public string Format { get; set; } = string.Empty;
    public string CommercialName { get; set; } = string.Empty;
    public string CommercialCompany { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty; 
    public string ScientificName { get; set; } = string.Empty;
    public int QuantityWarehouse { get; set; }
    public int QuantityNurse { get; set; }
    public int MinQuantityWarehouse { get; set; }
    public int MinQuantityNurse { get; set; }
    public int MaxQuantityWarehouse { get; set; }
    public int MaxQuantityNurse { get; set; }
}
