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
                .NotEmpty().WithMessage("El diagnóstico es requerido.")
                .MaximumLength(1000).WithMessage("El diagnóstico no puede exceder los 1000 caracteres.");

            RuleFor(x => x.EmergencyRoomId)
                .NotEmpty().WithMessage("La guardia es requerida");

            RuleFor(x => x.CareDate)
                .NotEmpty().WithMessage("La fecha de atencion es requerida")
                .Must(BeAValidDate).WithMessage("La fecha de atencion debe ser válida.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("El paciente es requerido");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default;
        }
    }
}