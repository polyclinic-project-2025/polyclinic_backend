using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;

public class CreateConsultationDerivationValidator : AbstractValidator<CreateConsultationDerivationDto>
{
    public CreateConsultationDerivationValidator(IConsultationDerivationRepository repository)
    {
        RuleFor(x => x.Diagnosis)
            .NotEmpty().WithMessage("El diagnóstico es requerido.")
            .MaximumLength(1000).WithMessage("El diagnóstico no puede exceder los 1000 caracteres.");

        RuleFor(x => x.DerivationId)
            .NotEmpty().WithMessage("El ID de derivación es requerido.");

        RuleFor(x => x.DateTimeCDer)
            .NotEmpty().WithMessage("La fecha de consulta es requerida.")
            .Must(BeAValidDate).WithMessage("La fecha de consulta debe ser válida.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("El ID del doctor es requerido.");

        RuleFor(x => x.DepartmentHeadId)
            .NotEmpty().WithMessage("El ID del jefe de departamento es requerido.");
    }
    private bool BeAValidDate(DateTime date)
    {
        return date != default;
    }
}