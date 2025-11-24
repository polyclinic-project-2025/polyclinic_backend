using System.ComponentModel.DataAnnotations;

namespace PolyclinicApplication.DTOs.Request;

public record UpdateUserDto
{
    public string? Property { get; set; }
    public string? Value { get; set; }
    
    public string? Operation { get; set; } // "add", "remove", "replace"
    public IList<string>? Roles { get; set; }
}