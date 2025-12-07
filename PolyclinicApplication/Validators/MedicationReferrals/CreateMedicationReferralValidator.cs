using FluentValidation;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators.MedicationReferrals;

public class CreateMedicationReferralValidator : AbstractValidator<CreateMedicationReferralDto>
{
    private readonly IConsultationReferralRepository _consultationReferralRepository;
    private readonly IMedicationRepository _medicationRepository;

    public CreateMedicationReferralValidator(
        IConsultationReferralRepository consultationReferralRepository,
        IMedicationRepository medicationRepository)
    {
        _consultationReferralRepository = consultationReferralRepository;
        _medicationRepository = medicationRepository;

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor que 0.")
            .LessThanOrEqualTo(1000000)
            .WithMessage("La cantidad no puede exceder 1,000,000.");

        RuleFor(x => x.ConsultationReferralId)
            .NotEmpty()
            .WithMessage("El ID de la consulta de remision es obligatorio.")
            .Must(ConsultationReferralExists)
            .WithMessage("La consulta de remision especificada no existe.");

        RuleFor(x => x.MedicationId)
            .NotEmpty()
            .WithMessage("El ID del medicamento es obligatorio.")
            .Must(MedicationExists)
            .WithMessage("El medicamento especificado no existe.");
    }

    private bool ConsultationReferralExists(Guid id)
    {
        var consultation = _consultationReferralRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        return consultation != null;
    }

    private bool MedicationExists(Guid id)
    {
        var medication = _medicationRepository.GetByIdAsync(id).GetAwaiter().GetResult();
        return medication != null;
    }
}
