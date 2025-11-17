namespace PolyclinicDomain.Entities;

public class Nursing
{
    public Guid Id { get; private set; }
    public string? Name { get; private set; }

    // Relación 1:1 con NursingHead
    public NursingHead? Head { get; private set; }
    public Guid? HeadId { get; private set; }

    public ICollection<Nurse> Nurses { get; set; } = new List<Nurse>();

    public Nursing(Guid id, string name, Guid? headId = null)
    {
        Id = id;
        Name = name;
        HeadId = headId;
    }

    // Constructor sin parámetros para EF Core
    private Nursing() { }

    public void AssignHead(Guid headId)
    {
        HeadId = headId;
    }

    public void RemoveHead()
    {
        HeadId = null;
    }
}