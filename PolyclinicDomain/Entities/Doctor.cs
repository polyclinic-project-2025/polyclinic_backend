using PolyclinicCore;
namespace PolyclinicDomain.Entities;

public class Doctor : MedicalStaff
{
    public ICollection<EmergencyRoom>? EmergencyRooms { get; set; }
    public Doctor(Guid id, string name, RoleUser role, string employmentStatus, int identification, Guid departmentId)
        : base(id, name, role, employmentStatus, identification, departmentId)
    {
    }
}
