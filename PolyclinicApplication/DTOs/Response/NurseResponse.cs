namespace Application.DTOs.Response
{
    public record NurseResponseDto(
        Guid NursingId,
        string? NursingName,
        string? UserId
    ) : EmployeeResponseDto(Guid.Empty, "", "", "", "");
}
