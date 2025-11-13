using System;

namespace PolyclinicApplication.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        // Campos opcionales para partial updates
        public string? Name { get; set; }
        public Guid? HeadId { get; set; } // null = remove head, value = set head, omitted = keep
    }
}
