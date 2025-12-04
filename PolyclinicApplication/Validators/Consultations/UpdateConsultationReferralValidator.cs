using FluentValidation;
using PolyclinicApplication.DTOs.Request.Consultations;

namespace PolyclinicApplication.Validators.Consultations;

public class UpdateConsultationReferralValidator : AbstractValidator<UpdateConsultationReferralDto>
{
    public UpdateConsultationReferralValidator()
    {
        When(x => x.ReferralId.HasValue, () =>
        {
            RuleFor(x => x.ReferralId!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("El ID del paciente remitido no puede ser vacío");
        });

        When(x => x.DoctorId.HasValue, () =>
        {
            RuleFor(x => x.DoctorId!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("El ID del doctor no puede ser vacío");
        });

        When(x => x.DateTimeCRem.HasValue, () =>
        {
            RuleFor(x => x.DateTimeCRem!.Value)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha no puede ser futura");
        });

        When(x => x.DepartmentHeadId.HasValue, () =>
        {
            RuleFor(x => x.DepartmentHeadId!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("El ID del jefe de departamento no puede ser vacío");
        });

        When(x => !string.IsNullOrEmpty(x.Diagnosis), () =>
        {
            RuleFor(x => x.Diagnosis)
                .MinimumLength(10)
                .WithMessage("El diagnóstico debe tener al menos 10 caracteres");
        });
    }
}