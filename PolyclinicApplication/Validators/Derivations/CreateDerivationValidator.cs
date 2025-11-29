using FluentValidation;
using PolyclinicApplication.DTOs.Request.Derivations;

namespace PolyclinicApplication.Validators.Derivations{
    public class CreateDerivationValidator : AbstractValidator<CreateDerivationDto>
{
    public CreateDerivationValidator()
    {
        RuleFor(x => x.DepartmentFromId)
            .NotEmpty().WithMessage("El departamento de origen es obligatorio.");

        RuleFor(x => x.DepartmentToId)
            .NotEmpty().WithMessage("El departamento de destino es obligatorio.");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("El paciente es obligatorio.");

        RuleFor(x => x.DateTimeDer)
            .NotEmpty().WithMessage("La fecha de derivación es obligatoria.");
    }
}

}

