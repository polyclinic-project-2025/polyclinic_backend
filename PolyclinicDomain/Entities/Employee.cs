
namespace PolyclinicDomain.Entities;

public enum RoleUser
{
    
}

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public RoleUser Role { get; private set; }
    public string EmploymentStatus { get; private set; }

    public string Password { get; private set; }

    public Employee(Guid id, string name, RoleUser role, string employmentStatus, string password)
    {
        Id = id;
        Name = name;
        Role = role;
        EmploymentStatus = employmentStatus;
        Password = password;
    }
}