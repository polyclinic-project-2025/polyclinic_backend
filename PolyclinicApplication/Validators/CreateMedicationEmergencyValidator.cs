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
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.EmergencyRoomCareId)
                .NotEmpty().WithMessage("EmergencyRoomCareId is required.");

            RuleFor(x => x.MedicationId)
                .NotEmpty().WithMessage("MedicationId is required.");
        }
    }
}