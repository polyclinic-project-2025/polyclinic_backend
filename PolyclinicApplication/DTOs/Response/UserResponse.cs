using System;

namespace PolyclinicApplication.DTOs.Response
{
    public record UserResponse
    {
        public string? Id { get; set; } = string.Empty;
        public string? Email {get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Identification { get; set; } = string.Empty;
        public IList<string>? Roles { get; set; } = new List<string>();
    }
}