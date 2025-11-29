using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class ExternalMedicalPost
{
    public Guid ExternalMedicalPostId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; }
    [Required]
    [MaxLength(500)]
    public string Address { get; set; }
    public ExternalMedicalPost(){}
    public ExternalMedicalPost(Guid externalMedicalPostId, string name, string address)
    {
        ExternalMedicalPostId = externalMedicalPostId;
        Name = name;
        Address = address;
    }
}