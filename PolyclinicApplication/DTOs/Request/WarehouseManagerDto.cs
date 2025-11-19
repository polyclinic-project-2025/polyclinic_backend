namespace Application.DTOs.Request
{
    public record WarehouseManagerDto(
        string Identification,
        string Name,
        string EmploymentStatus,
        Guid? ManagedWarehouseId
    ) : EmployeeDto(Identification, Name, EmploymentStatus);
}
