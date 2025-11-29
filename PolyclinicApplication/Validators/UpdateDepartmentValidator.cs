using FluentValidation;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicApplication.Validators.Departments
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name cannot be empty when provided.")
                    .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
            });
        }
    }
}
