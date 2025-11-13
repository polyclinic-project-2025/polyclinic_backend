namespace PolyclinicDomain.Entities;

public class ExternalMedicalPost
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; set; }

    public ExternalMedicalPost(Guid id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
    }
}