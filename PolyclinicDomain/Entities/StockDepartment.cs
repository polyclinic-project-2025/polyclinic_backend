using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class StockDepartment{
    public Guid StockDepartmentId { get; private set; }

    [Required]
    [Range(0, 1000000)]
    public int Quantity { get; private set; }
    
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    public Guid MedicationId { get; private set;}
    public Medication? Medication { get; private set; }

    public StockDepartment(
        Guid stockDepartmentId,
        int quantity,
        Guid departmentId,
        Guid medicationId)
    {
        StockDepartmentId = stockDepartmentId;
        Quantity = quantity;
        DepartmentId = departmentId;
        MedicationId = medicationId;
    }
}