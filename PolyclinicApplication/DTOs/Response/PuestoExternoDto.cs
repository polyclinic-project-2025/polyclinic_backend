using System;

namespace PolyclinicApplication.DTOs.Response
{
    public class PuestoExternoDto
    {
        public Guid PuestoExternoId { get; set; }
        public string? Name { get; set; }
        public string Address {get; set;}
    }
}