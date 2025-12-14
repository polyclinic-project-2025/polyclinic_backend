using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class CreateMedicationEmergencyValidator : AbstractValidator<CreateMedicationEmergencyDto>
    {
        public CreateMedicationEmergencyValidator(IMedicationEmergencyRepository repository)
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0.");

            RuleFor(x => x.EmergencyRoomCareId)
                .NotEmpty().WithMessage("La atencion de Cuerpo de Guardia es requerida");

            RuleFor(x => x.MedicationId)
                .NotEmpty().WithMessage("El medicamento es requerido");
        }
    }
}