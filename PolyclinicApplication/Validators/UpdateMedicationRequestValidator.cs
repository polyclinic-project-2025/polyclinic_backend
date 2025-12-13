using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Validators;

public class UpdateMedicationRequestValidator : AbstractValidator<UpdateMedicationRequestRequest>
{
    public UpdateMedicationRequestValidator()
    {
        RuleFor(wr => wr.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0.");
            
        RuleFor(wr => wr.Quantity)
            .LessThan(1000000)
            .WithMessage("La cantidad debe ser menor a 1,000,000.");
    }
}