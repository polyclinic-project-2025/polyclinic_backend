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
    private readonly IMedicationDerivationRepository _medicationDerivationRepository;
    private readonly IConsultationDerivationRepository _consultationDerivationRepository;
    private readonly IStockDepartmentRepository _stockDepartmentRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMedicationDerivationDto> _createValidator;
    private readonly IValidator<UpdateMedicationDerivationDto> _updateValidator;

    public MedicationDerivationService(
        IMedicationDerivationRepository medicationDerivationRepository,
        IConsultationDerivationRepository consultationDerivationRepository,
        IStockDepartmentRepository stockDepartmentRepository,
        IMapper mapper,
        IValidator<CreateMedicationDerivationDto> createValidator,
        IValidator<UpdateMedicationDerivationDto> updateValidator)
    {
        _medicationDerivationRepository = medicationDerivationRepository;
        _consultationDerivationRepository = consultationDerivationRepository;
        _stockDepartmentRepository = stockDepartmentRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<MedicationDerivationDto>> CreateAsync(CreateMedicationDerivationDto request)
    {
        // 1. Validar DTO
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<MedicationDerivationDto>.Failure(validation.Errors.First().ErrorMessage);

        // 2. Obtener la ConsultationDerivation con el Department
        var consultationDerivation = await _consultationDerivationRepository
            .GetWithDepartmentAsync(request.ConsultationDerivationId);
        
        if (consultationDerivation == null)
            return Result<MedicationDerivationDto>.Failure("La consulta de derivación no fue encontrada.");

        if (consultationDerivation.DepartmentHead?.Department == null)
            return Result<MedicationDerivationDto>.Failure("No se pudo obtener el departamento de la consulta.");

        var departmentId = consultationDerivation.DepartmentHead.Department.DepartmentId;

        // 3. Obtener el stock del departamento para el medicamento
        var stock = await _stockDepartmentRepository
            .GetByDepartmentAndMedicationAsync(departmentId, request.MedicationId);

        if (stock == null)
            return Result<MedicationDerivationDto>.Failure(
                "No existe stock del medicamento en el departamento especificado.");

        // 4. Validar que hay suficiente cantidad en stock
        if (stock.Quantity < request.Quantity)
            return Result<MedicationDerivationDto>.Failure(
                $"Stock insuficiente. Disponible: {stock.Quantity}, Solicitado: {request.Quantity}");

        // 5. Disminuir el stock
        stock.UpdateQuantity(stock.Quantity - request.Quantity);
        await _stockDepartmentRepository.UpdateAsync(stock);

        // 6. Crear la MedicationDerivation
        var medicationDerivation = new MedicationDerivation(
            Guid.NewGuid(),
            request.Quantity,
            request.ConsultationDerivationId,
            request.MedicationId
        );

        medicationDerivation = await _medicationDerivationRepository.AddAsync(medicationDerivation);

        var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
        return Result<MedicationDerivationDto>.Success(response);
    }

    public async Task<Result<MedicationDerivationDto>> GetByIdAsync(Guid id)
    {
        var medicationDerivation = await _medicationDerivationRepository.GetByIdAsync(id);
        if (medicationDerivation == null)
            return Result<MedicationDerivationDto>.Failure("La derivación de medicamento no fue encontrada.");

        var response = _mapper.Map<MedicationDerivationDto>(medicationDerivation);
        return Result<MedicationDerivationDto>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationDerivationDto>>> GetAllAsync()
    {
        var medicationDerivations = await _medicationDerivationRepository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<MedicationDerivationDto>>(medicationDerivations);
        return Result<IEnumerable<MedicationDerivationDto>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationDerivationDto request)
    {
        // 1. Validar DTO
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        // 2. Obtener la MedicationDerivation existente
        var medicationDerivation = await _medicationDerivationRepository.GetByIdAsync(id);
        if (medicationDerivation == null)
            return Result<bool>.Failure("La derivación de medicamento no fue encontrada.");

        // 3. Si se actualiza la cantidad, ajustar el stock
        if (request.Quantity.HasValue && request.Quantity.Value != medicationDerivation.Quantity)
        {
            // 3.1. Obtener la ConsultationDerivation con el Department
            var consultationDerivation = await _consultationDerivationRepository
                .GetWithDepartmentAsync(medicationDerivation.ConsultationDerivationId);
            
            if (consultationDerivation?.DepartmentHead?.Department == null)
                return Result<bool>.Failure("No se pudo obtener el departamento de la consulta.");

            var departmentId = consultationDerivation.DepartmentHead.Department.DepartmentId;

            // 3.2. Obtener el stock
            var stock = await _stockDepartmentRepository
                .GetByDepartmentAndMedicationAsync(departmentId, medicationDerivation.MedicationId);

            if (stock == null)
                return Result<bool>.Failure("No existe stock del medicamento en el departamento.");

            // 3.3. Calcular la diferencia
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

        // 4. Actualizar otros campos si vienen en el request
        if (request.ConsultationDerivationId.HasValue)
            medicationDerivation.UpdateConsultationDerivationId(request.ConsultationDerivationId.Value);

        if (request.MedicationId.HasValue)
            medicationDerivation.UpdateMedicationId(request.MedicationId.Value);

        await _medicationDerivationRepository.UpdateAsync(medicationDerivation);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        // 1. Obtener la MedicationDerivation
        var medicationDerivation = await _medicationDerivationRepository.GetByIdAsync(id);
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
        await _medicationDerivationRepository.DeleteAsync(medicationDerivation);
        return Result<bool>.Success(true);
    }
}