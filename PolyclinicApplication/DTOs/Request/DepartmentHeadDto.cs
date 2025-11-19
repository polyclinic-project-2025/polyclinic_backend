namespace Application.DTOs.Request
{
    public record CreateDepartmentHeadDto(
        string Identification,
        string Name,
        string EmploymentStatus,
        Guid ManagedDepartmentId
    );
}