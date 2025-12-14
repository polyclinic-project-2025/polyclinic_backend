using AutoMapper;
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
    private readonly IConsultationDerivationRepository _consultationDerivationRepository;
    private readonly IStockDepartmentRepository _stockDepartmentRepository;
    private readonly IMapper _mapper;

    public MedicationDerivationService(
        IMedicationDerivationRepository repository,
        IConsultationDerivationRepository consultationDerivationRepository,
        IStockDepartmentRepository stockDepartmentRepository,
        IMapper mapper)
    {
        _repository = repository;
        _consultationDerivationRepository = consultationDerivationRepository;
        _stockDepartmentRepository = stockDepartmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<MedicationDerivationDto>> CreateAsync(CreateMedicationDerivationDto request)
    {
        try
        {
            // 1. Obtener la ConsultationDerivation con el Department
            var consultationDerivation = await _consultationDerivationRepository
                .GetWithDepartmentAsync(request.ConsultationDerivationId);

            if (consultationDerivation == null)
                return Result<MedicationDerivationDto>.Failure("La consulta de derivación no fue encontrada.");

            if (consultationDerivation.DepartmentHead?.Department == null)
                return Result<MedicationDerivationDto>.Failure("No se pudo obtener el departamento de la consulta.");

            var departmentId = consultationDerivation.DepartmentHead.Department.DepartmentId;

            // 2. Obtener el stock del departamento para el medicamento
            var stock = await _stockDepartmentRepository
                .GetByDepartmentAndMedicationAsync(departmentId, request.MedicationId);

            if (stock == null)
                return Result<MedicationDerivationDto>.Failure(
                    "No existe stock del medicamento en el departamento especificado.");

            // 3. Validar que hay suficiente cantidad en stock
            if (stock.Quantity < request.Quantity)
                return Result<MedicationDerivationDto>.Failure(
                    $"Stock insuficiente. Disponible: {stock.Quantity}, Solicitado: {request.Quantity}");

            // 4. Disminuir el stock
            stock.UpdateQuantity(stock.Quantity - request.Quantity);
            await _stockDepartmentRepository.UpdateAsync(stock);

            // 5. Crear la MedicationDerivation
            var medicationDerivation = new MedicationDerivation(
                Guid.NewGuid(),
                request.Quantity,
                request.ConsultationDerivationId,
                request.MedicationId
            );

            medicationDerivation = await _repository.AddAsync(medicationDerivation);

            var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
            return Result<MedicationDerivationDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationDerivationDto>.Failure($"Error al guardar la derivación: {ex.Message}");
        }
    }

    public async Task<Result<MedicationDerivationDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var medicationDerivation = await _repository.GetByIdAsync(id);
            if (medicationDerivation == null)
                return Result<MedicationDerivationDto>.Failure("La derivación de medicamento no fue encontrada.");

            var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
            return Result<MedicationDerivationDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<MedicationDerivationDto>.Failure($"Error al obtener la derivación: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MedicationDerivationDto>>> GetAllAsync()
    {
        try
        {
            var medicationDerivations = await _repository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<MedicationDerivationDto>>(medicationDerivations);
            return Result<IEnumerable<MedicationDerivationDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MedicationDerivationDto>>.Failure($"Error al obtener las derivaciones: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDerivationDto request)
    {
        try
        {
            // 1. Obtener la MedicationDerivation existente
            var medicationDerivation = await _repository.GetByIdAsync(id);
            if (medicationDerivation == null)
                return Result<bool>.Failure("La derivación de medicamento no fue encontrada.");

            // 2. Si se actualiza la cantidad, ajustar el stock
            if (request.Quantity.HasValue && request.Quantity.Value != medicationDerivation.Quantity)
            {
                // 2.1. Obtener la ConsultationDerivation con el Department
                var consultationDerivation = await _consultationDerivationRepository
                    .GetWithDepartmentAsync(medicationDerivation.ConsultationDerivationId);

                if (consultationDerivation?.DepartmentHead?.Department == null)
                    return Result<bool>.Failure("No se pudo obtener el departamento de la consulta.");

                var departmentId = consultationDerivation.DepartmentHead.Department.DepartmentId;

                // 2.2. Obtener el stock
                var stock = await _stockDepartmentRepository
                    .GetByDepartmentAndMedicationAsync(departmentId, medicationDerivation.MedicationId);

                if (stock == null)
                    return Result<bool>.Failure("No existe stock del medicamento en el departamento.");

                // 2.3. Calcular la diferencia
                int difference = request.Quantity.Value - medicationDerivation.Quantity;

                if (difference > 0) // Se AUMENTÓ la cantidad recetada
                {
                    // Validar que hay suficiente stock adicional
                    if (stock.Quantity < difference)
                        return Result<bool>.Failure(
                            $"Stock insuficiente para aumentar la cantidad. Disponible: {stock.Quantity}, Necesario adicional: {difference}");

                    // Disminuir más stock
                    stock.UpdateQuantity(stock.Quantity - difference);
                }
                else if (difference < 0) // Se DISMINUYÓ la cantidad recetada
                {
                    // Devolver stock
                    stock.UpdateQuantity(stock.Quantity + Math.Abs(difference));
                }

                await _stockDepartmentRepository.UpdateAsync(stock);
                medicationDerivation.UpdateQuantity(request.Quantity.Value);
            }

            // 3. Actualizar otros campos si vienen en el request
            if (request.ConsultationDerivationId.HasValue)
                medicationDerivation.UpdateConsultationDerivationId(request.ConsultationDerivationId.Value);

            if (request.MedicationId.HasValue)
                medicationDerivation.UpdateMedicationId(request.MedicationId.Value);

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
        try
        {
            // 1. Obtener la MedicationDerivation
            var medicationDerivation = await _repository.GetByIdAsync(id);
            if (medicationDerivation == null)
                return Result<bool>.Failure("La derivación de medicamento no fue encontrada.");

            // 2. Devolver el stock al departamento
            var consultationDerivation = await _consultationDerivationRepository
                .GetWithDepartmentAsync(medicationDerivation.ConsultationDerivationId);

            if (consultationDerivation?.DepartmentHead?.Department != null)
            {
                var departmentId = consultationDerivation.DepartmentHead.Department.DepartmentId;

                var stock = await _stockDepartmentRepository
                    .GetByDepartmentAndMedicationAsync(departmentId, medicationDerivation.MedicationId);

                if (stock != null)
                {
                    // Devolver la cantidad al stock
                    stock.UpdateQuantity(stock.Quantity + medicationDerivation.Quantity);
                    await _stockDepartmentRepository.UpdateAsync(stock);
                }
            }

            // 3. Eliminar la MedicationDerivation
            await _repository.DeleteAsync(medicationDerivation);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar la derivación: {ex.Message}");
        }
    }
}