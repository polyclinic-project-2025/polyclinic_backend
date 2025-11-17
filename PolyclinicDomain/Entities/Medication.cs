namespace PolyclinicDomain.Entities;

public class Medication
{
    public Guid IdMed { get; private set; }
    public string Format { get; private set; }
    public string CommercialName { get; private set; }
    public string CommercialCompany { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public string BatchNumber { get; private set; }
    public string ScientificName { get; private set; }
    public int QuantityA { get; private set; }
    public int QuantityNurse { get; private set; }
    public ICollection<MedicationDerivation> ConsultationDer {get; set;}
    public ICollection<MedicationReferral> ConsultationRem {get;set;}
    public ICollection<MedicationEmergency> Emergency {get; set;}
    public ICollection<StockDepartment> Stock {get;set;}

    public Medication(
        Guid idMed,
        string format,
        string commercialName,
        string commercialCompany,
        DateTime expirationDate,
        string batchNumber,
        string scientificName,
        int quantityA,
        int quantityNurse)
    {
        IdMed = idMed;
        Format = format;
        CommercialName = commercialName;
        CommercialCompany = commercialCompany;
        ExpirationDate = expirationDate;
        BatchNumber = batchNumber;
        ScientificName = scientificName;
        QuantityA = quantityA;
        QuantityNurse = quantityNurse;
    }
}