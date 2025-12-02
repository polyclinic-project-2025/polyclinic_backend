using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Nursing
{
    public Guid NursingId { get; private set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; }

    public virtual ICollection<Nurse> Nurses { get; private set; } = new List<Nurse>();

    public Nursing(Guid nursingId, string name)
    {
        NursingId = nursingId;
        Name = name;
    }

    protected Nursing() { }
}