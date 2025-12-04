using FluentValidation;
using PolyclinicApplication.DTOs.Request.Patients;

namespace PolyclinicApplication.Validators.Patients
{
    public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
    {
        public CreatePatientValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre debe tener como máximo 200 caracteres.");

            RuleFor(x => x.Identification)
                .NotEmpty().WithMessage("La identificación es obligatoria.")
                .MaximumLength(50).WithMessage("La identificación debe tener como máximo 50 caracteres.");

            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("La edad es obligatoria.")
                .InclusiveBetween(0, 150).WithMessage("La edad debe estar entre 0 y 150 años.");

            RuleFor(x => x.Contact)
                .NotEmpty().WithMessage("El contacto es obligatorio.")
                .MaximumLength(100).WithMessage("El contacto debe tener como máximo 100 caracteres.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(500).WithMessage("La dirección debe tener como máximo 500 caracteres.");
        }
    }
}
