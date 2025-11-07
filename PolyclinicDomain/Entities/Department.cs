namespace PolyclinicDomain.Entities;

public class Department
{
    public Guid Id { get; private set; }
    public string? Name { get; private set; }

    // Relación 1:1 con DepartmentHead
    public DepartmentHead? Head { get; private set; }
    public Guid? HeadId { get; private set; }

    public ICollection<MedicalStaff> MedicalStaff { get; private set; } = new List<MedicalStaff>();
    public ICollection<StockDepartment> Stock { get; set; } = new List<StockDepartment>();

    public Department(Guid id, string name, Guid? headId = null)
    {
        Id = id;
        Name = name;
        HeadId = headId;
    }

    // Constructor sin parámetros para EF Core
    private Department() { }

    public void AssignHead(Guid headId)
    {
        HeadId = headId;
    }

    public void RemoveHead()
    {
        HeadId = null;
    }
}
