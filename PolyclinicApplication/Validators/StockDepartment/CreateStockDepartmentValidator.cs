using FluentValidation;
using PolyclinicApplication.DTOs.Request.StockDepartment;

namespace PolyclinicApplication.Validators.StockDepartment
{
    public class CreateStockDepartmentValidator : AbstractValidator<CreateStockDepartmentDto>
    {
        public CreateStockDepartmentValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad debe ser mayor o igual a 0.");

            RuleFor(x => x.MinQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("El minimo debe ser mayor o igual a 0.");

            RuleFor(x => x.MaxQuantity)
                .GreaterThanOrEqualTo(x => x.MinQuantity).WithMessage("El max");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("DepartmentId is required.");

            RuleFor(x => x.MedicationId)
                .NotEmpty().WithMessage("MedicationId is required.");
        }
    }
}