using System.ComponentModel.DataAnnotations;

public class BossResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public int Identification { get; set; }
}