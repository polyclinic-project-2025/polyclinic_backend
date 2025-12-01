using FluentValidation;
using PolyclinicApplication.DTOs.Request.Referral;

namespace PolyclinicApplication.Validators.Referral{
    public class CreateReferralValidator : AbstractValidator<CreateReferralDto>
{
    public CreateReferralValidator()
    {
        RuleFor(x => x.PuestoExterno)
            .NotEmpty().WithMessage("El puesto externo es obligatorio.");

        RuleFor(x => x.DepartmentToId)
            .NotEmpty().WithMessage("El departamento de destino es obligatorio.");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("El paciente es obligatorio.");

        RuleFor(x => x.DateTimeRem)
            .NotEmpty().WithMessage("La fecha de remision es obligatoria.");
    }
}

}