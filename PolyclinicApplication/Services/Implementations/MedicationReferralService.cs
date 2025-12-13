using AutoMapper;
using FluentValidation;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.DTOs.Response.MedicationReferrals;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class MedicationReferralService : IMedicationReferralService
{
    private readonly IMedicationReferralRepository _medicationReferralRepository;
    private readonly IConsultationReferralRepository _consultationReferralRepository;
    private readonly IStockDepartmentRepository _stockDepartmentRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMedicationReferralDto> _createValidator;
    private readonly IValidator<UpdateMedicationReferralDto> _updateValidator;

    public MedicationReferralService(
        IMedicationReferralRepository medicationReferralRepository,
        IConsultationReferralRepository consultationReferralRepository,
        IStockDepartmentRepository stockDepartmentRepository,
        IMapper mapper,
        IValidator<CreateMedicationReferralDto> createValidator,
        IValidator<UpdateMedicationReferralDto> updateValidator)
    {
        _medicationReferralRepository = medicationReferralRepository;
        _consultationReferralRepository = consultationReferralRepository;
        _stockDepartmentRepository = stockDepartmentRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<MedicationReferralDto>> CreateAsync(CreateMedicationReferralDto request)
    {
        // 1. Validar DTO
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<MedicationReferralDto>.Failure(validation.Errors.First().ErrorMessage);

        // 2. Obtener la ConsultationReferral con el Department
        var consultationReferral = await _consultationReferralRepository
            .GetWithDepartmentAsync(request.ConsultationReferralId);

        if(consultationReferral == null)
            return Result<MedicationReferralDto>.Failure("La consulta de remisión no fue encontrada.");

        if (consultationReferral.DepartmentHead?.Department == null)
            return Result<MedicationReferralDto>.Failure("No se pudo obtener el departamento de la consulta.");    
        
        var departmentId = consultationReferral.DepartmentHead.Department.DepartmentId;

        // 3. Obtener el stock del departamento para el medicamento
        var stock = await _stockDepartmentRepository
            .GetByDepartmentAndMedicationAsync(departmentId, request.MedicationId);

        if (stock == null)
            return Result<MedicationReferralDto>.Failure(
                "No existe stock del medicamento en el departamento especificado.");
        //4. Validar que hay suficiente cantidad en stock 
        if (stock.Quantity < request.Quantity)
            return Result<MedicationReferralDto>.Failure(
                $"Stock insuficiente. Disponible: {stock.Quantity}, Solicitado: {request.Quantity}");

        //5. Disminuir el stock
        stock.UpdateQuantity(stock.Quantity - request.Quantity);
        await _stockDepartmentRepository.UpdateAsync(stock);

        //6. Crear la MedicationReferral
        var medicationReferral = new MedicationReferral(
            Guid.NewGuid(),
            request.Quantity,
            request.ConsultationReferralId,
            request.MedicationId
        );

        medicationReferral = await _medicationReferralRepository.AddAsync(medicationReferral);

        var response = _mapper.Map<MedicationReferralDto>(medicationReferral);
        return Result<MedicationReferralDto>.Success(response);
    }

    public async Task<Result<MedicationReferralDto>> GetByIdAsync(Guid id)
    {
        var medicationReferral = await _medicationReferralRepository.GetByIdAsync(id);
        if (medicationReferral == null)
            return Result<MedicationReferralDto>.Failure("La remisión de medicamento no fue encontrada.");

        var response = _mapper.Map<MedicationReferralDto>(medicationReferral);
        return Result<MedicationReferralDto>.Success(response);
    }

    public async Task<Result<IEnumerable<MedicationReferralDto>>> GetAllAsync()
    {
        var medicationReferrals = await _medicationReferralRepository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<MedicationReferralDto>>(medicationReferrals);
        return Result<IEnumerable<MedicationReferralDto>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateMedicationReferralDto request)
    {
        //1. Validar DTO
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        //2. Obtener la remisión de medicamento existente
        var medicationReferral = await _medicationReferralRepository.GetByIdAsync(id);
        if (medicationReferral == null)
            return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

        //3. Si se actualiza la cantidad, ajustar el stock
        if(request.Quantity.HasValue && request.Quantity.Value != medicationReferral.Quantity)
        {
            var consultationReferral = await _consultationReferralRepository
                .GetWithDepartmentAsync(medicationReferral.ConsultationReferralId);

            if (consultationReferral == null)
                return Result<bool>.Failure("La consulta de remisión no fue encontrada.");

            if (consultationReferral.DepartmentHead?.Department == null)
                return Result<bool>.Failure("No se pudo obtener el departamento de la consulta.");

            var departmentId = consultationReferral.DepartmentHead.Department.DepartmentId;

            var stock = await _stockDepartmentRepository
                .GetByDepartmentAndMedicationAsync(departmentId, medicationReferral.MedicationId);

            if (stock == null)
                return Result<bool>.Failure(
                    "No existe stock del medicamento en el departamento especificado.");

            var quantityDifference = request.Quantity.Value - medicationReferral.Quantity;

            if(quantityDifference > 0)
            {
                if (stock.Quantity < quantityDifference)
                    return Result<bool>.Failure(
                        $"Stock insuficiente. Disponible: {stock.Quantity}, Solicitado: {quantityDifference}");

                stock.UpdateQuantity(stock.Quantity - quantityDifference);        
            }
            else if (quantityDifference < 0)
            {
                stock.UpdateQuantity(stock.Quantity + Math.Abs(quantityDifference));
            }

            await _stockDepartmentRepository.UpdateAsync(stock);

            medicationReferral.UpdateQuantity(request.Quantity.Value);
        }


        if(request.ConsultationReferralId.HasValue)
            medicationReferral.UpdateConsultationReferralId(request.ConsultationReferralId.Value);
            
        if(request.MedicationId.HasValue)
            medicationReferral.UpdateMedicationId(request.MedicationId.Value);
    
        await _medicationReferralRepository.UpdateAsync(medicationReferral);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var medicationReferral = await _medicationReferralRepository.GetByIdAsync(id);
        if (medicationReferral == null)
            return Result<bool>.Failure("La remisión de medicamento no fue encontrada.");

        await _medicationReferralRepository.DeleteAsync(medicationReferral);
        return Result<bool>.Success(true);
    }
}
