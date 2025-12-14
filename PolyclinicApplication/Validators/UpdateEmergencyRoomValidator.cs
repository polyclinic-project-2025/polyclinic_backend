using FluentValidation;
using PolyclinicApplication.DTOs.Request;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators
{
    public class UpdateEmergencyRoomValidator : AbstractValidator<UpdateEmergencyRoomDto>
    {
        public UpdateEmergencyRoomValidator(IEmergencyRoomRepository repository)
        {
            // Cuando DoctorId no es null, validarlo
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("El doctor es requerido");

            // Cuando GuardDate no es null, validarlo
            RuleFor(x => x.GuardDate)
                .NotEmpty().WithMessage("La fecha de guardia es requerida")
                .Must(BeAValidDate).WithMessage("La fecha de guardia debe ser válida")
                .When(x => x.GuardDate.HasValue);
        }

        private bool BeAValidDate(DateOnly? date)
        {
            if (!date.HasValue) return true;  // Si es null, es válido (porque el When ya filtró)
            return date.Value != default;
        }
    }
}