namespace Application.DTOs.Response
{
    public record NursingHeadResponseDto(
        Guid? ManagedNursingId,
        string? ManagedNursingName
    ) : EmployeeResponseDto(Guid.Empty, "", "", "", "");
}
