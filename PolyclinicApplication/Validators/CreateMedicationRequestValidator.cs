using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Validators;

public class CreateMedicationRequestValidator : AbstractValidator<CreateMedicationRequestRequest>
{
    public CreateMedicationRequestValidator()
    {
        RuleFor(wr => wr.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(wr => wr.Quantity)
            .LessThan(1000000)
            .WithMessage("La cantidad debe ser menor a 1,000,000.");

        RuleFor(wr => wr.WarehouseRequestId)
            .NotEmpty()
            .WithMessage("El ID de la solicitud al almacén es requerido.");
        
        RuleFor(wr => wr.MedicationId)
            .NotEmpty()
            .WithMessage("El ID del medicamento es requerido.");
    }
}