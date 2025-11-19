namespace Application.DTOs.Response
{
    public record DoctorResponseDto(
        // ICollection<Guid>? EmergencyRoomIds = null
    ) : MedicalStaffResponseDto(Guid.Empty, null)
    {
    }
}
