using AutoMapper;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class UnifiedConsultationService : IUnifiedConsultationService
{
    private readonly IConsultationDerivationRepository _derivationRepo;
    private readonly IConsultationReferralRepository _referralRepo;
    private readonly IMedicationDerivationRepository _medicationDerivationRepo;
    private readonly IMedicationReferralRepository _medicationReferralRepo;
    private readonly IMapper _mapper;

    public UnifiedConsultationService(
        IConsultationDerivationRepository derivationRepo,
        IConsultationReferralRepository referralRepo,
        IMedicationDerivationRepository medicationDerivationRepo,
        IMedicationReferralRepository medicationReferralRepo,
        IMapper mapper)
    {
        _derivationRepo = derivationRepo;
        _referralRepo = referralRepo;
        _medicationDerivationRepo = medicationDerivationRepo;
        _medicationReferralRepo = medicationReferralRepo;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UnifiedConsultationDto>>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        try
        {
            var derivations = await _derivationRepo.GetLast10ByPatientIdAsync(patientId);
            var referrals = await _referralRepo.GetLast10ByPatientIdAsync(patientId);

            var unifiedList = new List<UnifiedConsultationDto>();
            
            // Mapear derivaciones
            var derivationDtos = _mapper.Map<IEnumerable<UnifiedConsultationDto>>(derivations);
            unifiedList.AddRange(derivationDtos);
            
            // Mapear remisiones
            var referralDtos = _mapper.Map<IEnumerable<UnifiedConsultationDto>>(referrals);
            unifiedList.AddRange(referralDtos);

            // Cargar medicamentos para cada consulta
            await LoadMedicationsForConsultations(unifiedList);

            // Ordenar y tomar las últimas 10
            var result = unifiedList
                .OrderByDescending(c => c.Date)
                .Take(10)
                .ToList();

            return Result<IEnumerable<UnifiedConsultationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UnifiedConsultationDto>>.Failure($"Error al obtener consultas: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<UnifiedConsultationDto>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return Result<IEnumerable<UnifiedConsultationDto>>.Failure(
                    "La fecha de inicio no puede ser mayor que la fecha de fin.");

            var derivations = await _derivationRepo.GetByDateRangeAsync(patientId, startDate, endDate);
            var referrals = await _referralRepo.GetByDateRangeAsync(patientId, startDate, endDate);

            var unifiedList = new List<UnifiedConsultationDto>();
            
            // Mapear derivaciones
            var derivationDtos = _mapper.Map<IEnumerable<UnifiedConsultationDto>>(derivations);
            unifiedList.AddRange(derivationDtos);
            
            // Mapear remisiones
            var referralDtos = _mapper.Map<IEnumerable<UnifiedConsultationDto>>(referrals);
            unifiedList.AddRange(referralDtos);

            // Cargar medicamentos para cada consulta
            await LoadMedicationsForConsultations(unifiedList);

            // Ordenar por fecha descendente
            var result = unifiedList
                .OrderByDescending(c => c.Date)
                .ToList();

            return Result<IEnumerable<UnifiedConsultationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UnifiedConsultationDto>>.Failure($"Error al obtener consultas: {ex.Message}");
        }
    }

    private async Task LoadMedicationsForConsultations(List<UnifiedConsultationDto> consultations)
    {
        try
        {
            foreach (var consultation in consultations)
            {
                if (consultation.Type == "Derivation")
                {
                    // Obtener medicamentos de derivación
                    var medications = await _medicationDerivationRepo.GetByConsultationDerivationIdAsync(consultation.Id);
                    
                    consultation.Medications = medications.Select(m => new MedicationInfoDto
                    {
                        MedicationId = m.MedicationId,
                        Name = m.Medication?.CommercialName ?? "Desconocido",
                        Quantity = m.Quantity
                    }).ToList();
                }
                else if (consultation.Type == "Referral")
                {
                    // Obtener medicamentos de remisión
                    var medications = await _medicationReferralRepo.GetByConsultationReferralIdAsync(consultation.Id);
                    
                    consultation.Medications = medications.Select(m => new MedicationInfoDto
                    {
                        MedicationId = m.MedicationId,
                        Name = m.Medication?.CommercialName ?? "Desconocido",
                        Quantity = m.Quantity
                    }).ToList();
                }
            }
        }
        catch (Exception)
        {
            // En caso de error, dejar la lista de medicamentos vacía
            foreach (var consultation in consultations)
            {
                consultation.Medications = new List<MedicationInfoDto>();
            }
        }
    }
}