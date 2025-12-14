using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class UpdateMedicationEmergencyValidator : AbstractValidator<UpdateMedicationEmergencyDto>
    {
        public UpdateMedicationEmergencyValidator(IMedicationEmergencyRepository repository)
        {
            // Cuando Quantity no es null, validarlo
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0.");

            // Cuando EmergencyRoomCareId no es null, validarlo
            RuleFor(x => x.EmergencyRoomCareId)
                .NotEmpty().WithMessage("La atencion de Cuerpo de Guardia es requerida");

            // Cuando MedicationId no es null, validarlo
            RuleFor(x => x.MedicationId)
                .NotEmpty().WithMessage("El medicamento es requerido");
        }
    }
}