using System;

namespace PolyclinicApplication.DTOs.Departments
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? HeadId { get; set; }
    }
}
