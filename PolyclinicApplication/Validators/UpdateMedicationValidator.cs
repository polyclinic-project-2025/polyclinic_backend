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
            .NotEmpty().WithMessage("El formato es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.CommercialName)
            .NotEmpty().WithMessage("El nombre comercial es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.CommercialCompany)
            .NotEmpty().WithMessage("La compañía comercial es requerida.")
            .MaximumLength(100);

        RuleFor(x => x.ScientificName)
            .NotEmpty().WithMessage("El nombre científico es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.ExpirationDate)
            .NotEmpty()
            .Must(date => DateOnly.TryParse(date, out _))
            .WithMessage("La fecha de expiración debe ser válida en formato yyyy-MM-dd.");


        // Cantidades actuales
        RuleFor(x => x.QuantityWarehouse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad en almacén no puede ser negativa.");

        RuleFor(x => x.QuantityNurse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad en enfermería no puede ser negativa.");
    }
}
