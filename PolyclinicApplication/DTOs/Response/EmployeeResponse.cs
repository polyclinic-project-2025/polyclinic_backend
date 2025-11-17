namespace Application.DTOs.Response
{
    public record EmployeeResponseDto(
        Guid Id,
        string Identification,
        string Name,
        string EmploymentStatus,
        string PrimaryRole
    );
}
