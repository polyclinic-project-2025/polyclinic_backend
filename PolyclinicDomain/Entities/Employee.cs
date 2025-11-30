using System.ComponentModel.DataAnnotations;

namespace PolyclinicDomain.Entities;

public abstract class Employee
{
    public Guid EmployeeId { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Identification { get; private set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; }
    [Required]
    [MaxLength(50)]
    public string EmploymentStatus { get; private set; }

    public string? UserId { get; set; } = null;

    protected Employee(Guid employeeId, string identification, string name, string employmentStatus)
    {
        EmployeeId = employeeId;
        Identification = identification;
        Name = name;
        EmploymentStatus = employmentStatus;
    }

    protected Employee() { }

    public abstract string GetPrimaryRole();

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateIdentification(string identification)
    {
        Identification = identification;
    }

    public void UpdateEmploymentStatus(string employmentStatus)
    {
        EmploymentStatus = employmentStatus;
    }
}