using FluentValidation;
using PolyclinicApplication.DTOs.Request.Auth;

namespace PolyclinicApplication.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
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
            .WithMessage("La contraseña es obligatoria")
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches(@"[A-Z]")
            .WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]")
            .WithMessage("La contraseña debe contener al menos una letra minúscula")
            .Matches(@"[0-9]")
            .WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Debe confirmar la contraseña")
            .Equal(x => x.Password)
            .WithMessage("Las contraseñas no coinciden");

        When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("El número de teléfono no es válido");
        });

        RuleFor(x => x.Roles)
            .NotEmpty()
            .WithMessage("Indique al menos un rol de usuario")
            .Must(roles => roles != null && roles.Any())
            .WithMessage("Debe proporcionar al menos un rol");
    }
}