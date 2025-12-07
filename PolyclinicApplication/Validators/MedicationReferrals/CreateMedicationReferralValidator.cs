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
            .WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Quantity cannot exceed 1,000,000.");

        RuleFor(x => x.ConsultationReferralId)
            .NotEmpty()
            .WithMessage("ConsultationReferralId is required.")
            .MustAsync(async (id, cancellation) => await ConsultationReferralExistsAsync(id))
            .WithMessage("The specified ConsultationReferral does not exist.");

        RuleFor(x => x.MedicationId)
            .NotEmpty()
            .WithMessage("MedicationId is required.")
            .MustAsync(async (id, cancellation) => await MedicationExistsAsync(id))
            .WithMessage("The specified Medication does not exist.");
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
