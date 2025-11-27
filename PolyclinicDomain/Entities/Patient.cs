using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Patient
{
    public Guid PatientId { get; private set; } // ID_Pac
    
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; } // Nombre_Pac
    [Required]
    [MaxLength(50)]
    public string Identification { get; private set; }
    [Required]
    [Range(0, 150)]
    public int Age { get; private set; } // Edad
    [Required]
    [MaxLength(100)]
    public string Contact { get; private set; } // Contacto 
    [Required]
    [MaxLength(500)]
    public string Address { get; private set; } // Dirección_Pac
    
    public ICollection<Derivation> Derivations { get; private set; } = new List<Derivation>();
    public ICollection<Referral> Referrals { get; private set; } = new List<Referral>();
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>();
    public ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>();
    public ICollection<EmergencyRoomCare> EmergencyRoomCares { get; private set; } = new List<EmergencyRoomCare>();
    
    public string? UserId { get; set; }

    public Patient(Guid patientId, string name, string identification, int age, string contact, string address)
    {
        PatientId = patientId;
        Name = name;
        Identification = identification;
        Age = age;
        Contact = contact;
        Address = address;
    }
}
