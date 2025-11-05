using PolyclinicCore;
namespace PolyclinicDomain.Entities;

public class Boss : Employee
{
    public Boss(Guid id, string name, RoleUser role, string employmentStatus, int identification)
        : base(id, name, role, employmentStatus, identification)
    {
    }
}
