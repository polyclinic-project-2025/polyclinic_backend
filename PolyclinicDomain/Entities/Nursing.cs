namespace PolyclinicDomain.Entities;

public class Nursing
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Boss? Boss { get; private set; }
    public Guid BossId { get; private set; }
    public ICollection<Nurse>? Nurses { get; set; }

    public Nursing(Guid id, string name, Guid bossId)
    {
        Id = id;
        Name = name;
        BossId = bossId;
    }
}