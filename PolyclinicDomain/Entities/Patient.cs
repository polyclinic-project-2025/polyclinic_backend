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
    
    public virtual ICollection<Derivation> Derivations { get; private set; } = new List<Derivation>();
    public virtual ICollection<Referral> Referrals { get; private set; } = new List<Referral>();
    public virtual ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>();
    public virtual ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>();
    public virtual ICollection<EmergencyRoomCare> EmergencyRoomCares { get; private set; } = new List<EmergencyRoomCare>();
    
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

    protected Patient(){}
    // -------------------------------
        // MÉTODOS DE CAMBIO
        // -------------------------------

        public void ChangeName(string name)
        {
            Name = name;
        }

        public void ChangeIdentification(string identification)
        {
            Identification = identification;
        }

        public void ChangeAge(int age)
        {
            Age = age;
        }

        public void ChangeContact(string contact)
        {
            Contact = contact;
        }

        public void ChangeAddress(string address)
        {
            Address = address;
        }
}
