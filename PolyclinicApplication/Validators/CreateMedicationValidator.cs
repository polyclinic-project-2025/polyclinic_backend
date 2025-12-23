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

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("El número de lote es requerido.")
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

        // Límites mínimos
        RuleFor(x => x.MinQuantityWarehouse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad mínima en almacén debe ser mayor o igual a 0.");

        RuleFor(x => x.MinQuantityNurse)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La cantidad mínima en enfermería debe ser mayor o igual a 0.");

        // Límites máximos
        RuleFor(x => x.MaxQuantityWarehouse)
            .GreaterThan(x => x.MinQuantityWarehouse)
            .WithMessage("La cantidad máxima en almacén debe ser mayor que la cantidad mínima.");

        RuleFor(x => x.MaxQuantityNurse)
            .GreaterThan(x => x.MinQuantityNurse)
            .WithMessage("La cantidad máxima en enfermería debe ser mayor que la cantidad mínima.");

        // Validación lógica final (cantidades actuales dentro del rango)
        RuleFor(x => x)
            .Must(x => x.QuantityWarehouse <= x.MaxQuantityWarehouse)
            .WithMessage("La cantidad en almacén no puede exceder la cantidad máxima.")
            .Must(x => x.QuantityWarehouse >= x.MinQuantityWarehouse)
            .WithMessage("La cantidad en almacén no puede estar por debajo de la cantidad mínima.")
            .Must(x => x.QuantityNurse <= x.MaxQuantityNurse)
            .WithMessage("La cantidad en enfermería no puede exceder la cantidad máxima.")
            .Must(x => x.QuantityNurse >= x.MinQuantityNurse)
            .WithMessage("La cantidad en enfermería no puede estar por debajo de la cantidad mínima.");         
    }
}