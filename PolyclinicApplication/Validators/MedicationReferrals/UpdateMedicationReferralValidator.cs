using FluentValidation;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Validators.MedicationReferrals;

public class UpdateMedicationReferralValidator : AbstractValidator<UpdateMedicationReferralDto>
{
    private readonly IConsultationReferralRepository _consultationReferralRepository;
    private readonly IMedicationRepository _medicationRepository;

    public UpdateMedicationReferralValidator(
        IConsultationReferralRepository consultationReferralRepository,
        IMedicationRepository medicationRepository)
    {
        _consultationReferralRepository = consultationReferralRepository;
        _medicationRepository = medicationRepository;

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor que 0.")
            .LessThanOrEqualTo(1000000)
            .WithMessage("La cantidad no puede exceder 1,000,000.")
            .When(x => x.Quantity.HasValue);

        RuleFor(x => x.ConsultationReferralId)
            .NotEmpty()
            .WithMessage("El ID de la consulta de remision es obligatorio.")
            .MustAsync(async (id, cancellation) => await ConsultationReferralExistsAsync(id!.Value))
            .WithMessage("La consulta de remision especificada no existe.")
            .When(x => x.ConsultationReferralId.HasValue);

        RuleFor(x => x.MedicationId)
            .NotEmpty()
            .WithMessage("El ID del medicamento es obligatorio.")
            .MustAsync(async (id, cancellation) => await MedicationExistsAsync(id!.Value))
            .WithMessage("El medicamento especificado no existe.")
            .When(x => x.MedicationId.HasValue);
    }

    private async Task<bool> ConsultationReferralExistsAsync(Guid id)
    {
        var consultation = await _consultationReferralRepository.GetByIdAsync(id);
        return consultation != null;
    }

    private async Task<bool> MedicationExistsAsync(Guid id)
    {
        var medication = await _medicationRepository.GetByIdAsync(id);
        return medication != null;
    }
}
