using PolyclinicCore;

namespace PolyclinicDomain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public RoleUser Role { get; private set; }
    public string EmploymentStatus { get; private set; }

    public Employee(Guid id, string name, RoleUser role, string employmentStatus)
    {
        Id = id;
        Name = name;
        Role = role;
        EmploymentStatus = employmentStatus;
    }
}