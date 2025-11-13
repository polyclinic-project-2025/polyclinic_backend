using System;

namespace PolyclinicApplication.DTOs.Departments
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? HeadId { get; set; }
    }
}
