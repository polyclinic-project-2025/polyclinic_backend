using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class CreateEmergencyRoomValidator : AbstractValidator<CreateEmergencyRoomDto>
    {
        public CreateEmergencyRoomValidator(IEmergencyRoomRepository repository)
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("DoctorId is required.");

            RuleFor(x => x.GuardDate)
                .NotEmpty().WithMessage("GuardDate is required.")
                .Must(BeAValidDate).WithMessage("GuardDate must be a valid date.");
        }

        private bool BeAValidDate(DateOnly date)
        {
            return date != default;
        }
    }
}