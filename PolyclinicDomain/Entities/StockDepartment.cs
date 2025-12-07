using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class StockDepartment{
    public Guid StockDepartmentId { get; private set; }

    [Required]
    [Range(0, 1000000)]
    public int Quantity { get; private set; }
    
    public Guid DepartmentId { get; private set; }
    public virtual Department? Department { get; private set; }

    public Guid MedicationId { get; private set;}
    public virtual Medication? Medication { get; private set; }

    public int MinQuantity {get; private set;}
    public int MaxQuantity {get; private set;}
    public StockDepartment(
        Guid stockDepartmentId,
        int quantity,
        Guid departmentId,
        Guid medicationId,
        int minQuantity,
        int maxQuantity)
    {
        StockDepartmentId = stockDepartmentId;
        Quantity = quantity;
        DepartmentId = departmentId;
        MedicationId = medicationId;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
    }

    protected StockDepartment(){}

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    public void UpdateMinQuantity(int minQuantity)
    {
        MinQuantity = minQuantity;
    }

    public void UpdateMaxQuantity(int maxQuantity)
    {
        MaxQuantity = maxQuantity;
    }
}