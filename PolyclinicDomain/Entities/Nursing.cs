namespace PolyclinicDomain.Entities;

public class Nursing
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public ICollection<Nurse> Nurses { get; set; }

    public Nursing(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}