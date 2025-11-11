namespace PolyclinicDomain.Entities;

public class Patient
{
    public Guid PatientId { get; private set; } // ID_Pac
    public string Name { get; private set; } // Nombre_Pac
    public int Identification { get; private set; }
    public int Age { get; private set; } // Edad
    public string Contact { get; private set; } // Contacto 
    public string Address { get; private set; } // Dirección_Pac
    public string? UserId { get; set; }
    public ICollection<Derivation> Derivations { get; set; } = new List<Derivation>();
    public ICollection<Referral> Referrals { get; set; } = new List<Referral>();
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; set; } = new List<ConsultationDerivation>();
    public ICollection<ConsultationReferral> ConsultationReferrals { get; set; } = new List<ConsultationReferral>();

    public Patient(Guid patientId, string name, int identification, int age, string contact, string address)
    {
        PatientId = patientId;
        Name = name;
        Identification = identification;
        Age = age;
        Contact = contact;
        Address = address;
    }
}
