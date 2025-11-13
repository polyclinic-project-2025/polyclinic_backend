using PolyclinicCore.Constants;

namespace PolyclinicDomain.Entities;

/// <summary>
/// Representa un Doctor en el sistema.
/// Un Doctor puede tener roles adicionales como DepartmentHead mediante EmployeeRoles.
/// </summary>
public class Doctor : MedicalStaff
{
    public ICollection<EmergencyRoom>? EmergencyRooms { get; set; }

    public Doctor(Guid id, string name, string employmentStatus, string identification, Guid departmentId)
        : base(id, name, employmentStatus, identification, departmentId)
    {
    }

    // Constructor sin parámetros para EF Core
    private Doctor() { }

    public override string GetPrimaryRole() => ApplicationRoles.Doctor;
}
