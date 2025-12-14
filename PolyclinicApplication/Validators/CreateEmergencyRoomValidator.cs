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
                .NotEmpty().WithMessage("El doctor es requerido");

            RuleFor(x => x.GuardDate)
                .NotEmpty().WithMessage("La fecha de guardia es requerida")
                .Must(BeAValidDate).WithMessage("La fecha de guardia debe ser válida");
        }

        private bool BeAValidDate(DateOnly date)
        {
            return date != default;
        }
    }
}