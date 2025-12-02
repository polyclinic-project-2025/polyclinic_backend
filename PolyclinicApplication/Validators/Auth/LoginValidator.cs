using FluentValidation;
using PolyclinicApplication.DTOs.Request.Auth;

namespace PolyclinicApplication.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .EmailAddress()
            .WithMessage("El email no es válido")
            .MaximumLength(256)
            .WithMessage("El email no puede exceder los 256 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria");
    }
}