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

    protected Employee(Guid employeeId, string identification, string name, string employmentStatus)
    {
        EmployeeId = employeeId;
        Identification = identification;
        Name = name;
        EmploymentStatus = employmentStatus;
    }

    protected Employee() { }

    public abstract string GetPrimaryRole();
}