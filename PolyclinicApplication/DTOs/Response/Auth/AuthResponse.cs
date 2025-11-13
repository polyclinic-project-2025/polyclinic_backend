using System.ComponentModel.DataAnnotations;

namespace PolyclinicApplication.DTOs.Response.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        
    }
}