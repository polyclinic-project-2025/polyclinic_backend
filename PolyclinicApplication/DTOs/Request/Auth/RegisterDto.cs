using System.ComponentModel.DataAnnotations;

namespace PolyclinicApplication.DTOs.Request.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Indique al menos un rol de usuario.")]
        public IList<string> Roles { get; set; } = new List<string>();

        public Dictionary<string, string>? ValidationData { get; set; } = new Dictionary<string, string>();

    }
}
