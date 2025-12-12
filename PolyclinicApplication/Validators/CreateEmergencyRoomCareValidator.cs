using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class CreateEmergencyRoomCareValidator : AbstractValidator<CreateEmergencyRoomCareDto>
    {
        public CreateEmergencyRoomCareValidator(IEmergencyRoomCareRepository repository)
        {
            RuleFor(x => x.Diagnosis)
                .NotEmpty().WithMessage("Diagnosis is required.")
                .MaximumLength(1000).WithMessage("Diagnosis cannot exceed 1000 characters.");

            RuleFor(x => x.EmergencyRoomId)
                .NotEmpty().WithMessage("EmergencyRoomId is required.");

            RuleFor(x => x.CareDate)
                .NotEmpty().WithMessage("CareDate is required.")
                .Must(BeAValidDate).WithMessage("CareDate must be a valid date.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("PatientId is required.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default;
        }
    }
}