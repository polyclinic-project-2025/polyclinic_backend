using AutoMapper;
using FluentValidation;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class MedicationDerivationService : IMedicationDerivationService
{
    private readonly IMedicationDerivationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMedicationDerivationDto> _createValidator;
    private readonly IValidator<UpdateMedicationDerivationDto> _updateValidator;

    public MedicationDerivationService(
        IMedicationDerivationRepository repository,
        IMapper mapper,
        IValidator<CreateMedicationDerivationDto> createValidator,
        IValidator<UpdateMedicationDerivationDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<MedicationDerivationDto>> CreateAsync(CreateMedicationDerivationDto request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<MedicationDerivationDto>.Failure(validation.Errors.First().ErrorMessage);

        var medicationDerivation = new MedicationDerivation(
            Guid.NewGuid(),
            request.Quantity,
            request.ConsultationDerivationId,
            request.MedicationId
        );

        try
        {
            medicationDerivation = await _repository.AddAsync(medicationDerivation);
        }
        catch (Exception ex)
        {
            return Result<MedicationDerivationDto>.Failure($"Error al guardar la derivación: {ex.Message}");
        }

        var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
        return Result<MedicationDerivationDto>.Success(response);
    }

    public async Task<Result<MedicationDerivationDto>> GetByIdAsync(Guid id)
    {
        var medicationDerivation = await _repository.GetByIdAsync(id);
        if (medicationDerivation == null)
            return Result<MedicationDerivationDto>.Failure("La derivación de medicamento no fue encontrada.");

        var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
        return Result<MedicationDerivationDto>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDerivationDto>>> GetAllAsync()
    {
        var medicationDerivations = await _repository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<MedicationDerivationDto>>(medicationDerivations);
        return Result<IEnumerable<MedicationDerivationDto>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDerivationDto request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var medicationDerivation = await _repository.GetByIdAsync(id);
        if (medicationDerivation == null)
            return Result<bool>.Failure("La derivación de medicamento no fue encontrada.");

        if (request.Quantity > 0)
            medicationDerivation.UpdateQuantity(request.Quantity.Value);

        if (request.ConsultationDerivationId.HasValue)
            medicationDerivation.UpdateConsultationDerivationId(request.ConsultationDerivationId.Value);

        if (request.MedicationId.HasValue)
            medicationDerivation.UpdateMedicationId(request.MedicationId.Value);

        try
        {
            await _repository.UpdateAsync(medicationDerivation);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar la derivación: {ex.Message}");
        }                    
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var medicationDerivation = await _repository.GetByIdAsync(id);
        if (medicationDerivation == null)
            return Result<bool>.Failure("La derivación de medicamento no fue encontrada.");

        try
        {
            await _repository.DeleteAsync(medicationDerivation);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la derivación: {ex.Message}");
        }
    }
}