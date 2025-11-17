namespace Application.DTOs.Request
{
    public record NurseDto(
        Guid NursingId 
    ) : EmployeeDto(
        Identification: "",
        Name: "",
        EmploymentStatus: "" 
    );
}
