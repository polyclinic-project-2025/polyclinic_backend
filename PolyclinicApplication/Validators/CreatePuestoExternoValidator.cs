using System;
using FluentValidation;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Validators
{
    public class CreatePuestoExternoValidator : AbstractValidator<CreatePuestoExternoDto>
    {
        public CreatePuestoExternoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(500).WithMessage("Address must be at most 500 characters.");
        }
    }
}