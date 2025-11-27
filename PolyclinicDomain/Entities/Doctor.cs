using PolyclinicCore.Constants;
using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public class Doctor : MedicalStaff
{
    public ICollection<EmergencyRoom> EmergencyRooms { get; private set; } = new List<EmergencyRoom>();
    public ICollection<ConsultationDerivation> ConsultationDerivations { get; private set; } = new List<ConsultationDerivation>();
    public ICollection<ConsultationReferral> ConsultationReferrals { get; private set; } = new List<ConsultationReferral>();

    public Doctor(Guid doctorId, string name, string employmentStatus, string identification, Guid departmentId)
        : base(doctorId, name, employmentStatus, identification, departmentId)
    {
    }

    private Doctor() { }

    public override string GetPrimaryRole() => ApplicationRoles.Doctor;
}
