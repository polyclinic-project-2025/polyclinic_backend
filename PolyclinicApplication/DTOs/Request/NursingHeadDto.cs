namespace Application.DTOs.Request
{
    public record NursingHeadDto(
        Guid? ManagedNursingId
    ) : EmployeeDto(
        Identification: "",
        Name: "",
        EmploymentStatus: ""
    );
}
