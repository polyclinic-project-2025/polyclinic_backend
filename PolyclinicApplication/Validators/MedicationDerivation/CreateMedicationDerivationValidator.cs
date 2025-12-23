using FluentValidation;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators.MedicationDerivation
{
    public class CreateMedicationDerivationValidator : AbstractValidator<CreateMedicationDerivationDto>
    {
        private readonly IConsultationDerivationRepository _consultationDerivationRepository;
        private readonly IMedicationRepository _medicationRepository;

        public CreateMedicationDerivationValidator(
            IConsultationDerivationRepository consultationDerivationRepository,
            IMedicationRepository medicationRepository)
        {
            _consultationDerivationRepository = consultationDerivationRepository;
            _medicationRepository = medicationRepository;

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor que 0.");

            RuleFor(x => x.ConsultationDerivationId)
                .NotEmpty()
                .WithMessage("El ID de la consulta de derivación es obligatorio.")
                .Must(ConsultationDerivationExists)
                .WithMessage("La consulta de derivación especificada no existe.");

            RuleFor(x => x.MedicationId)
                .NotEmpty()
                .WithMessage("El ID del medicamento es obligatorio.")
                .Must(MedicationExists)
                .WithMessage("El medicamento especificado no existe.");
        }

        private bool ConsultationDerivationExists(Guid id)
        {
            var consultation = _consultationDerivationRepository.GetByIdAsync(id).GetAwaiter().GetResult();
            return consultation != null;
        }

        private bool MedicationExists(Guid id)
        {
            var medication = _medicationRepository.GetByIdAsync(id).GetAwaiter().GetResult();
            return medication != null;
        }
    }
}