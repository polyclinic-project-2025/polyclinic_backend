namespace Application.DTOs.Request
{
    public record DepartmentHeadDto(
        string Identification,
        string Name,
        string EmploymentStatus,
        Guid ManagedDepartmentId
    );
}