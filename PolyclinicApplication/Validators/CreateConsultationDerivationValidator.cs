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
            .NotEmpty().WithMessage("Diagnosis is required.")
            .MaximumLength(1000).WithMessage("Diagnosis cannot exceed 1000 characters.");

        RuleFor(x => x.DerivationId)
            .NotEmpty().WithMessage("DerivationId is required.");

        RuleFor(x => x.DateTimeCDer)
            .NotEmpty().WithMessage("DateTimeCDer is required.")
            .Must(BeAValidDate).WithMessage("DateTimeCDer must be a valid date.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required.");

        RuleFor(x => x.DepartmentHeadId)
            .NotEmpty().WithMessage("DepartmentHeadId is required.");
    }
    private bool BeAValidDate(DateTime date)
    {
        return date != default;
    }
}