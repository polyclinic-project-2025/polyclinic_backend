namespace Application.DTOs.Request
{
    public record DoctorDto(
        // List<Guid>? EmergencyRoomIds,
        Guid DepartmentId,
        string Identification,
        string Name,
        string EmploymentStatus
    ) : MedicalStaffDto(DepartmentId)
    {
    }
}
