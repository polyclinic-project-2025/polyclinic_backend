namespace Application.DTOs.Response
{
    /// <summary>
    /// DTO de respuesta con datos completos del jefe de departamento
    /// </summary>
    public record DepartmentHeadDto
    {
        public Guid Id { get; init; }
        public string Identification { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string EmploymentStatus { get; init; } = string.Empty;
        public string PrimaryRole { get; init; } = string.Empty;
        public Guid? ManagedDepartmentId { get; init; }
        public string? ManagedDepartmentName { get; init; }
        public string? UserId { get; init; }
    }
}