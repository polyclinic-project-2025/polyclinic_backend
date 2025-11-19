namespace Application.DTOs.Response
{
    public record MedicalStaffResponseDto(
        Guid DepartmentId,
        string? DepartmentName
    ) : EmployeeResponseDto(Guid.Empty, "", "", "", "");
}
