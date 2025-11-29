using System;

namespace PolyclinicApplication.DTOs.Request
{
    public class CreatePuestoExternoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address {get; set;} = string.Empty;
    }
}