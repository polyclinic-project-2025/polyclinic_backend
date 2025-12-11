using AutoMapper;
using FluentValidation;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.StockDepartment;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class StockDepartmentService : IStockDepartmentService
{
    private readonly IStockDepartmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateStockDepartmentDto> _createValidator;
    private readonly IValidator<UpdateStockDepartmentDto> _updateValidator;

    public StockDepartmentService(
        IStockDepartmentRepository repository,
        IMapper mapper,
        IValidator<CreateStockDepartmentDto> createValidator,
        IValidator<UpdateStockDepartmentDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<StockDepartmentDto>> CreateAsync(CreateStockDepartmentDto request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<StockDepartmentDto>.Failure(validation.Errors.First().ErrorMessage);

        var stockDepartment = new StockDepartment(
            Guid.NewGuid(),
            request.Quantity,
            request.DepartmentId,
            request.MedicationId,
            request.MinQuantity,
            request.MaxQuantity
        );

        stockDepartment = await _repository.AddAsync(stockDepartment);

        var response = _mapper.Map<StockDepartmentDto>(stockDepartment);
        return Result<StockDepartmentDto>.Success(response);
    }

    public async Task<Result<StockDepartmentDto>> GetByIdAsync(Guid id)
    {
        var stockDepartment = await _repository.GetByIdAsync(id);
        if (stockDepartment == null)
            return Result<StockDepartmentDto>.Failure("El stock del departamento no fue encontrado.");

        var response = _mapper.Map<StockDepartmentDto>(stockDepartment);
        return Result<StockDepartmentDto>.Success(response);
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetAllAsync()
    {
        var stockDepartments = await _repository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(stockDepartments);
        return Result<IEnumerable<StockDepartmentDto>>.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateStockDepartmentDto request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<bool>.Failure(validation.Errors.First().ErrorMessage);

        var stockDepartment = await _repository.GetByIdAsync(id);
        if (stockDepartment == null)
            return Result<bool>.Failure("El stock del departamento no fue encontrado.");

        if (request.Quantity >= 0)
            stockDepartment.UpdateQuantity(request.Quantity.Value);

        if (request.MinQuantity >= 0)
            stockDepartment.UpdateMinQuantity(request.MinQuantity.Value);

        if (request.MaxQuantity >= stockDepartment.MinQuantity)
            stockDepartment.UpdateMaxQuantity(request.MaxQuantity.Value); 

        await _repository.UpdateAsync(stockDepartment);
                   
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var stockDepartment = await _repository.GetByIdAsync(id);
        if (stockDepartment == null)
            return Result<bool>.Failure("El stock del departamento no fue encontrado.");

        await _repository.DeleteAsync(stockDepartment);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetStockByDepartmentIdAsync(Guid departmentId)
    {
        var stockDepartments = await _repository.GetStockByDepartmentAsync(departmentId);
        if (stockDepartments == null || !stockDepartments.Any())
            return Result<IEnumerable<StockDepartmentDto>>.Failure("No se encontró stock para el departamento especificado.");
        
        var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(stockDepartments);
        
        return Result<IEnumerable<StockDepartmentDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetLowStockByDepartmentIdAsync(Guid departmentId)
    {
        var lowStockDepartments = await _repository.GetBelowMinQuantityAsync(departmentId);
        
        var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(lowStockDepartments);
        
        return Result<IEnumerable<StockDepartmentDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetOverStockByDepartmentIdAsync(Guid departmentId)
    {
        var overStockDepartments = await _repository.GetAboveMaxQuantityAsync(departmentId);
        
        var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(overStockDepartments);
        
        return Result<IEnumerable<StockDepartmentDto>>.Success(response);
    }
}