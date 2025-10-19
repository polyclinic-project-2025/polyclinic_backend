
using System.Collections.Specialized;

namespace PolyclinicDomain.Entities;

public class Warehouse
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Employee? Boss { get; private set; }
    public Guid? BossId { get; private set; }

    public Warehouse(Guid id, string name, Guid? bossId)
    {
        Id = id;
        Name = name;
        BossId = bossId;
    }
}