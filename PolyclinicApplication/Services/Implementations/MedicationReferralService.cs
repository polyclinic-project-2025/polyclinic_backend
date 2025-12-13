using AutoMapper;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.DTOs.Response.MedicationReferrals;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class MedicationReferralService : IMedicationReferralService
{
    private readonly IMedicationReferralRepository _repository;
    private readonly IMapper _mapper;

    public MedicationReferralService(
        IMedicationReferralRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<MedicationReferralDto>> CreateAsync(CreateMedicationReferralDto request)
    {
        try
        {
            var medicationReferral = new MedicationReferral(
                Guid.NewGuid(),
                request.Quantity,
                request.ConsultationReferralId,
                request.MedicationId
            );

            medicationReferral = await _repository.AddAsync(medicationReferral);

            var response = _mapper.Map<MedicationReferralDto>(medicationReferral);
            return Result<MedicationReferralDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationReferralDto>.Failure($"Error al guardar la remisión: {ex.Message}");
        }
    }

    public async Task<Result<MedicationReferralDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var medicationReferral = await _repository.GetByIdAsync(id);
            if (medicationReferral == null)
                return Result<MedicationReferralDto>.Failure("La remisión de medicamento no fue encontrada.");

            var response = _mapper.Map<MedicationReferralDto>(medicationReferral);
            return Result<MedicationReferralDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationReferralDto>.Failure($"Error al obtener la remisión: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationReferralDto>>> GetAllAsync()
    {
        try
        {
            var medicationReferrals = await _repository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<MedicationReferralDto>>(medicationReferrals);
            return Result<IEnumerable<MedicationReferralDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationReferralDto>>.Failure($"Error al obtener las remisiones: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationReferralDto request)
    {
        try
        {
            var medicationReferral = await _repository.GetByIdAsync(id);
            if (medicationReferral == null)
                return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

            // Update only provided fields
            if (request.Quantity.HasValue)
            {
                medicationReferral.UpdateQuantity(request.Quantity.Value);
            }

            if (request.ConsultationReferralId.HasValue)
            {
                medicationReferral.UpdateConsultationReferralId(request.ConsultationReferralId.Value);
            }

            if (request.MedicationId.HasValue)
            {
                medicationReferral.UpdateMedicationId(request.MedicationId.Value);
            }

            await _repository.UpdateAsync(medicationReferral);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar la remisión: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var medicationReferral = await _repository.GetByIdAsync(id);
            if (medicationReferral == null)
                return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

            await _repository.DeleteAsync(medicationReferral);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la remisión: {ex.Message}");
        }
    }
}
