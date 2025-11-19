namespace Application.DTOs.Request
{
    public record EmployeeDto(
        string Identification,
        string Name,
        string EmploymentStatus
    );
}