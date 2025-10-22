namespace PolyclinicDomain.Entities;

public class Department
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Boss? Boss { get; private set; }
    public Guid BossId { get; private set; }

    public ICollection<MedicalStaff> MedicalStaff { get; private set; } = new List<MedicalStaff>();
    public ICollection<StockDepartment> Stock {get;set;} 

    public Department(Guid id, string name, Guid bossId)
    {
        Id = id;
        Name = name;
        BossId = bossId;
    }
}
