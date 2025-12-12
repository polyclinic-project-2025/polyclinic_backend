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
                .GreaterThan(0).WithMessage("Quantity must be greater than 0 if provided.");

            // Cuando EmergencyRoomCareId no es null, validarlo
            RuleFor(x => x.EmergencyRoomCareId)
                .NotEmpty().WithMessage("EmergencyRoomCareId cannot be empty if provided.");

            // Cuando MedicationId no es null, validarlo
            RuleFor(x => x.MedicationId)
                .NotEmpty().WithMessage("MedicationId cannot be empty if provided.");
        }
    }
}