using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Medication
{
    public Guid MedicationId { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Format { get; private set; }
    [Required]
    [MaxLength(100)]
    public string CommercialName { get; private set; }
    [Required]
    [MaxLength(100)]
    public string CommercialCompany { get; private set; }
    [Required]
    public DateTime ExpirationDate { get; private set; }
    [Required]
    [MaxLength(100)]
    public string BatchNumber { get; private set; }
    [Required]
    [MaxLength(100)]
    public string ScientificName { get; private set; }
    [Required]
    [Range(0, 1000000)]
    public int QuantityWarehouse { get; private set; }
    [Required]
    [Range(0, 1000000)]
    public int QuantityNurse { get; private set; }
    public int MinQuantityWarehouse { get; private set; } 
    public int MinQuantityNurse { get; private set; }
    public int MaxQuantityWarehouse { get; private set; }
    public int MaxQuantityNurse { get; private set; }
    
    public virtual ICollection<MedicationDerivation> MedicationDerivations { get; private set; } = new List<MedicationDerivation>();
    public virtual ICollection<MedicationReferral> MedicationReferrals { get; private set; } = new List<MedicationReferral>();
    public virtual ICollection<MedicationEmergency> MedicationEmergencies { get; private set; } = new List<MedicationEmergency>();
    public virtual ICollection<MedicationRequest> MedicationRequests { get; private set; } = new List<MedicationRequest>();
    public virtual ICollection<StockDepartment> StockDepartments { get; private set; } = new List<StockDepartment>();

    public Medication(
        Guid medicationId,
        string format,
        string commercialName,
        string commercialCompany,
        string batchNumber,
        string scientificName,
        int quantityWarehouse, // Renamed from quantityA
        int quantityNurse,
        int minQuantityWarehouse,
        int minQuantityNurse,
        int maxQuantityWarehouse,
        int maxQuantityNurse)
    {
        MedicationId = medicationId;   
        Format = format;
        CommercialName = commercialName;
        CommercialCompany = commercialCompany;
        BatchNumber = batchNumber;
        ScientificName = scientificName;
        QuantityWarehouse = quantityWarehouse; // Corrected property name
        QuantityNurse = quantityNurse;
        MinQuantityWarehouse = minQuantityWarehouse;
        MinQuantityNurse = minQuantityNurse;
        MaxQuantityWarehouse = maxQuantityWarehouse;
        MaxQuantityNurse = maxQuantityNurse;
    }

    public void UpdateFormat(string format)
    {
        Format = format;
    }

    public void UpdateCommercialName(string commercialName)
    {
        CommercialName = commercialName;
    }

    public void UpdateCommercialCompany(string commercialCompany)
    {
        CommercialCompany = commercialCompany;
    }

    public void UpdateExpirationDate(DateTime expirationDate)
    {
        ExpirationDate = expirationDate;
    }

    public void UpdateScientificName(string scientificName)
    {
        ScientificName = scientificName;
    }

    public void UpdateQuantityWarehouse(int quantityWarehouse)
    {
        QuantityWarehouse = quantityWarehouse;
    }

    public void UpdateQuantityNurse(int quantityNurse)
    {
        QuantityNurse = quantityNurse;
    }
    
}