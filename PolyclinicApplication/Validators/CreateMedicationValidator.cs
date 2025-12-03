using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators;
public class CreateMedicationValidator : AbstractValidator<CreateMedicationDto>
{
    public CreateMedicationValidator(IMedicationRepository repository)
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

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("Batch number is required.")
            .MaximumLength(100)
            .MustAsync(async (batch, _) => !await repository.ExistsBatchAsync(batch))
            .WithMessage("Batch number already exists.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future.");

        // Cantidades actuales
        RuleFor(x => x.QuantityWarehouse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("QuantityWarehouse cannot be negative.");

        RuleFor(x => x.QuantityNurse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("QuantityNurse cannot be negative.");

        // Límites mínimos
        RuleFor(x => x.MinQuantityWarehouse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MinQuantityWarehouse must be >= 0.");

        RuleFor(x => x.MinQuantityNurse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MinQuantityNurse must be >= 0.");

        // Límites máximos
        RuleFor(x => x.MaxQuantityWarehouse)
            .GreaterThan(x => x.MinQuantityWarehouse)
            .WithMessage("MaxQuantityWarehouse must be greater than MinQuantityWarehouse.");

        RuleFor(x => x.MaxQuantityNurse)
            .GreaterThan(x => x.MinQuantityNurse)
            .WithMessage("MaxQuantityNurse must be greater than MinQuantityNurse.");

        // Validación lógica final (cantidades actuales dentro del rango)
        RuleFor(x => x)
            .Must(x => x.QuantityWarehouse <= x.MaxQuantityWarehouse)
            .WithMessage("QuantityWarehouse cannot exceed MaxQuantityWarehouse.")
            .Must(x => x.QuantityWarehouse >= x.MinQuantityWarehouse)
            .WithMessage("QuantityWarehouse cannot be below MinQuantityWarehouse.")
            .Must(x => x.QuantityNurse <= x.MaxQuantityNurse)
            .WithMessage("QuantityNurse cannot exceed MaxQuantityNurse.")
            .Must(x => x.QuantityNurse >= x.MinQuantityNurse)
            .WithMessage("QuantityNurse cannot be below MinQuantityNurse.");         
    }
}