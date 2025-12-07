using FluentValidation;
using PolyclinicApplication.DTOs.Request.Consultations;

namespace PolyclinicApplication.Validators.Consultations;

public class CreateConsultationReferralValidator : AbstractValidator<CreateConsultationReferralDto>
{
    public CreateConsultationReferralValidator()
    {
        RuleFor(x => x.ReferralId)
            .NotEmpty()
            .WithMessage("Es requerido el paciente remitido")
            .NotEqual(Guid.Empty)
            .WithMessage("El ID del paciente remitido no puede ser vacío");

        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Es requerido el doctor tratante")
            .NotEqual(Guid.Empty)
            .WithMessage("El ID del doctor no puede ser vacío");

        RuleFor(x => x.DateTimeCRem)
            .NotEmpty()
            .WithMessage("La fecha es requerida")
            .LessThan(DateTime.Now)
            .WithMessage("La fecha no puede ser futura");

        RuleFor(x => x.DepartmentHeadId)
            .NotEmpty()
            .WithMessage("El jefe de departamento es requerido")
            .NotEqual(Guid.Empty)
            .WithMessage("El ID del jefe de departamento no puede ser vacío");

        RuleFor(x => x.Diagnosis)
            .NotEmpty()
            .WithMessage("Debe proporcionar un diagnóstico")
            .MinimumLength(10)
            .WithMessage("El diagnóstico debe tener al menos 10 caracteres");
    }
}