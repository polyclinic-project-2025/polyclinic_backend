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
                .NotEmpty().WithMessage("El diagnóstico es requerido.")
                .MaximumLength(1000).WithMessage("El diagnóstico no puede exceder los 1000 caracteres.");

            // Cuando EmergencyRoomId no es null, validarlo
            RuleFor(x => x.EmergencyRoomId)
                .NotEmpty().WithMessage("La guardia es requerida");

            // Cuando CareDate no es null, validarlo
            RuleFor(x => x.CareDate)
                .NotEmpty().WithMessage("La fecha de atencion es requerida")
                .Must(BeAValidDate).WithMessage("La fecha de atencion debe ser válida.")
                .When(x => x.CareDate.HasValue);

            // Cuando PatientId no es null, validarlo
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("El paciente es requerido");
        }

        private bool BeAValidDate(DateTime? date)
        {
            if (!date.HasValue) return true; // Si es null, es válido (porque el When ya filtró)
            return date.Value != default;
        }
    }
}