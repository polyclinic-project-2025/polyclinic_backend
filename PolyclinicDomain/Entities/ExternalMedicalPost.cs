using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ExternalMedicalPost
{
    public Guid ExternalMedicalPostId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; }
    public ExternalMedicalPost(Guid externalMedicalPostId, string name)
    {
        ExternalMedicalPostId = externalMedicalPostId;
        Name = name;
    }
}