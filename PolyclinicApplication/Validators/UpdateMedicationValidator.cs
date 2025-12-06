using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;
public class UpdateMedicationValidator : AbstractValidator<UpdateMedicationDto>
{
    public UpdateMedicationValidator()
    {
        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("Format is required.")
            .MaximumLength(100);

        RuleFor(x => x.CommercialName)
            .NotEmpty().WithMessage("Commercial name is required.")
            .MaximumLength(100);

        RuleFor(x => x.CommercialCompany)
            .NotEmpty().WithMessage("Commercial company is required.")
            .MaximumLength(100);

        RuleFor(x => x.ScientificName)
            .NotEmpty().WithMessage("Scientific name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ExpirationDate)
            .NotEmpty()
            .Must(date => DateOnly.TryParse(date, out _))
            .WithMessage("ExpirationDate must be a valid date in format yyyy-MM-dd.");


        // Cantidades actuales
        RuleFor(x => x.QuantityWarehouse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("QuantityWarehouse cannot be negative.");

        RuleFor(x => x.QuantityNurse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("QuantityNurse cannot be negative.");
    }
}
