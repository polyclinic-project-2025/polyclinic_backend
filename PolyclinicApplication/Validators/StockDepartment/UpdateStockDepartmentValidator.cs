using FluentValidation;
using PolyclinicApplication.DTOs.Request.StockDepartment;

namespace PolyclinicApplication.Validators.StockDepartment
{
    public class UpdateStockDepartmentValidator : AbstractValidator<UpdateStockDepartmentDto>
    {
        public UpdateStockDepartmentValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad debe ser mayor o igual a 0.");

            RuleFor(x => x.MinQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("El minimo debe ser mayor o igual a 0.");

            RuleFor(x => x.MaxQuantity)
                .GreaterThanOrEqualTo(x => x.MinQuantity).WithMessage("El maximo debe ser mayor o igual al minimo.");
        }
    }
}