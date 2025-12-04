using System;
using FluentValidation;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicApplication.Validators.Departments
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
        }
    }
}
