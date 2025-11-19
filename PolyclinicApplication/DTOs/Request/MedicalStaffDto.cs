namespace Application.DTOs.Request
{
    public record MedicalStaffDto(
        Guid DepartmentId
    ) : EmployeeDto(
        Identification: "",
        Name: "",
        EmploymentStatus: "activo"
    );
}
