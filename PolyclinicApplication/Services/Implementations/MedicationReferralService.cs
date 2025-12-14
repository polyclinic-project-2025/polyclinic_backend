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
    private readonly IConsultationReferralRepository _consultationReferralRepository;
    private readonly IStockDepartmentRepository _stockDepartmentRepository;
    private readonly IMapper _mapper;

    public MedicationReferralService(
        IMedicationReferralRepository repository,
        IConsultationReferralRepository consultationReferralRepository,
        IStockDepartmentRepository stockDepartmentRepository,
        IMapper mapper)
    {
        _repository = repository;
        _consultationReferralRepository = consultationReferralRepository;
        _stockDepartmentRepository = stockDepartmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<MedicationReferralDto>> CreateAsync(CreateMedicationReferralDto request)
    {
        try
        {
            // 1. Obtener la ConsultationReferral con el Department
            var consultationReferral = await _consultationReferralRepository
                .GetWithDepartmentAsync(request.ConsultationReferralId);

            if(consultationReferral == null)
                return Result<MedicationReferralDto>.Failure("La consulta de remisión no fue encontrada.");

            if (consultationReferral.DepartmentHead?.Department == null)
                return Result<MedicationReferralDto>.Failure("No se pudo obtener el departamento de la consulta.");

            var departmentId = consultationReferral.DepartmentHead.Department.DepartmentId;

            // 2. Obtener el stock del departamento para el medicamento
            var stock = await _stockDepartmentRepository
                .GetByDepartmentAndMedicationAsync(departmentId, request.MedicationId);

            if (stock == null)
                return Result<MedicationReferralDto>.Failure(
                    "No existe stock del medicamento en el departamento especificado.");

            // 3. Validar que hay suficiente cantidad en stock
            if (stock.Quantity < request.Quantity)
                return Result<MedicationReferralDto>.Failure(
                    $"Stock insuficiente. Disponible: {stock.Quantity}, Solicitado: {request.Quantity}");

            // 4. Disminuir el stock
            stock.UpdateQuantity(stock.Quantity - request.Quantity);
            await _stockDepartmentRepository.UpdateAsync(stock);

            // 5. Crear la MedicationReferral
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
            // 1. Obtener la MedicationReferral existente
            var medicationReferral = await _repository.GetByIdAsync(id);
            if (medicationReferral == null)
                return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

            // 2. Si se actualiza la cantidad, ajustar el stock
            if (request.Quantity.HasValue && request.Quantity.Value != medicationReferral.Quantity)
            {
                // 2.1. Obtener la ConsultationReferral con el Department
                var consultationReferral = await _consultationReferralRepository
                    .GetWithDepartmentAsync(medicationReferral.ConsultationReferralId);

                if (consultationReferral == null)
                    return Result<bool>.Failure("La consulta de remisión no fue encontrada.");

                if (consultationReferral.DepartmentHead?.Department == null)
                    return Result<bool>.Failure("No se pudo obtener el departamento de la consulta.");

                var departmentId = consultationReferral.DepartmentHead.Department.DepartmentId;
  
                // 2.2. Obtener el stock
                var stock = await _stockDepartmentRepository
                    .GetByDepartmentAndMedicationAsync(departmentId, medicationReferral.MedicationId);

                if (stock == null)
                    return Result<bool>.Failure(
                        "No existe stock del medicamento en el departamento especificado.");

                // 2.3. Calcular la diferencia
                var quantityDifference = request.Quantity.Value - medicationReferral.Quantity;

                if(quantityDifference > 0) // Se AUMENTÓ la cantidad recetada
                {
                    if (stock.Quantity < quantityDifference)
                        return Result<bool>.Failure(
                            $"Stock insuficiente para aumentar la cantidad. Disponible: {stock.Quantity}, Necesario adicional: {quantityDifference}");
                    
                    // Disminuir más stock
                    stock.UpdateQuantity(stock.Quantity - quantityDifference);
                }
                else if (quantityDifference < 0) // Se DISMINUYÓ la cantidad recetada
                {
                    // Devolver stock
                    stock.UpdateQuantity(stock.Quantity + Math.Abs(quantityDifference));
                }

                await _stockDepartmentRepository.UpdateAsync(stock);
                medicationReferral.UpdateQuantity(request.Quantity.Value);
            }

            // 3. Actualizar otros campos si vienen en el request
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
            // 1. Obtener la MedicationReferral
            var medicationReferral = await _repository.GetByIdAsync(id);
            if (medicationReferral == null)
                return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

            // 2. Devolver el stock al departamento
            var consultationReferral = await _consultationReferralRepository
                .GetWithDepartmentAsync(medicationReferral.ConsultationReferralId);

            if (consultationReferral?.DepartmentHead?.Department != null)
            {
                var departmentId = consultationReferral.DepartmentHead.Department.DepartmentId;

                var stock = await _stockDepartmentRepository
                    .GetByDepartmentAndMedicationAsync(departmentId, medicationReferral.MedicationId);

                if (stock != null)
                {
                    // Devolver la cantidad al stock
                    stock.UpdateQuantity(stock.Quantity + medicationReferral.Quantity);
                    await _stockDepartmentRepository.UpdateAsync(stock);
                }
            }

            // 3. Eliminar la MedicationReferral
            await _repository.DeleteAsync(medicationReferral);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la remisión: {ex.Message}");
        }
    }
}
