namespace Application.DTOs.Response
{
    public record WarehouseManagerResponseDto(
        Guid Id,
        string Identification,         
        string Name,                   
        string EmploymentStatus,       
        string PrimaryRole,                   
        Guid? ManagedWarehouseId
    ) : EmployeeResponseDto(Id, Identification, Name, EmploymentStatus, PrimaryRole);
}
