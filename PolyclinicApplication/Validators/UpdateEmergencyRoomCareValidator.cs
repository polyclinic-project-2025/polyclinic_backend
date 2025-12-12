using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class UpdateEmergencyRoomCareValidator : AbstractValidator<UpdateEmergencyRoomCareDto>
    {
        public UpdateEmergencyRoomCareValidator(IEmergencyRoomCareRepository repository)
        {
            // Cuando Diagnosis no es null, validarlo
            RuleFor(x => x.Diagnosis)
                .NotEmpty().WithMessage("Diagnosis cannot be empty if provided.");

            // Cuando EmergencyRoomId no es null, validarlo
            RuleFor(x => x.EmergencyRoomId)
                .NotEmpty().WithMessage("EmergencyRoomId cannot be empty if provided.");

            // Cuando CareDate no es null, validarlo
            RuleFor(x => x.CareDate)
                .NotEmpty().WithMessage("CareDate cannot be empty if provided.")
                .Must(BeAValidDate).WithMessage("CareDate must be a valid date.")
                .When(x => x.CareDate.HasValue);

            // Cuando PatientId no es null, validarlo
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("PatientId cannot be empty if provided.");
        }

        private bool BeAValidDate(DateTime? date)
        {
            if (!date.HasValue) return true; // Si es null, es válido (porque el When ya filtró)
            return date.Value != default;
        }
    }
}