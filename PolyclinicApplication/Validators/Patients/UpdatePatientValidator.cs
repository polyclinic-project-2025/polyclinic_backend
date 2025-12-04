using FluentValidation;
using PolyclinicApplication.DTOs.Request.Patients;

namespace PolyclinicApplication.Validators.Patients
{
    public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
    {
        public UpdatePatientValidator()
        {
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                    .MaximumLength(200).WithMessage("El nombre debe tener como máximo 200 caracteres.");
            });

            When(x => x.Identification != null, () =>
            {
                RuleFor(x => x.Identification)
                    .NotEmpty().WithMessage("La identificación no puede estar vacía.")
                    .MaximumLength(50).WithMessage("La identificación debe tener como máximo 50 caracteres.");
            });

            When(x => x.Age != null, () =>
            {
                RuleFor(x => x.Age)
                    .InclusiveBetween(0, 150).WithMessage("La edad debe estar entre 0 y 150 años.");
            });

            When(x => x.Contact != null, () =>
            {
                RuleFor(x => x.Contact)
                    .NotEmpty().WithMessage("El contacto no puede estar vacío.")
                    .MaximumLength(100).WithMessage("El contacto debe tener como máximo 100 caracteres.");
            });

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("La dirección no puede estar vacía.")
                    .MaximumLength(500).WithMessage("La dirección debe tener como máximo 500 caracteres.");
            });
        }
    }
}
