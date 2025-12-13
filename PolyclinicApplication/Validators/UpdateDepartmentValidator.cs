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
                    .NotEmpty().WithMessage("El nombre no puede estar vacío si se proporciona.")
                    .MaximumLength(100).WithMessage("El nombre debe tener como máximo 100 caracteres.");
            });
        }
    }
}
