using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Department
{
    public Guid DepartmentId { get; private set; }
    
    [Required]
    [MaxLength(200)] 
    public string Name { get; private set; }

    public ICollection<DepartmentHead> DepartmentHeads { get; private set; } = new List<DepartmentHead>();
    public ICollection<Derivation> DerivationsFrom { get; private set; } = new List<Derivation>();
    public ICollection<Derivation> DerivationsTo { get; private set; } = new List<Derivation>();
    public ICollection<Referral> Referrals { get; private set; } = new List<Referral>();
    public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();
    public ICollection<StockDepartment> StockDepartments { get; private set; } = new List<StockDepartment>();
    public ICollection<WarehouseRequest> WarehouseRequests { get; private set; } = new List<WarehouseRequest>();

    public Department(Guid departmentId, string name)
    {
        DepartmentId = departmentId;
        Name = name;
    }

    private Department() { }

    public void ChangeName(string name)
    {
        Name = name;
    }
}
