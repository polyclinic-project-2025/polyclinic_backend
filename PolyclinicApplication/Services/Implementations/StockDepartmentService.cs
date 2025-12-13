using AutoMapper;
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

    public StockDepartmentService(
        IStockDepartmentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<StockDepartmentDto>> CreateAsync(CreateStockDepartmentDto request)
    {
        try
        {
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
        catch (Exception ex)
        {
            return Result<StockDepartmentDto>.Failure($"Error al guardar el stock: {ex.Message}");
        }
    }

    public async Task<Result<StockDepartmentDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var stockDepartment = await _repository.GetByIdAsync(id);
            if (stockDepartment == null)
                return Result<StockDepartmentDto>.Failure("El stock del departamento no fue encontrado.");

            var response = _mapper.Map<StockDepartmentDto>(stockDepartment);
            return Result<StockDepartmentDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<StockDepartmentDto>.Failure($"Error al obtener el stock: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetAllAsync()
    {
        try
        {
            var stockDepartments = await _repository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(stockDepartments);
            return Result<IEnumerable<StockDepartmentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<StockDepartmentDto>>.Failure($"Error al obtener el stock: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateStockDepartmentDto request)
    {
        try
        {
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
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al actualizar el stock: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var stockDepartment = await _repository.GetByIdAsync(id);
            if (stockDepartment == null)
                return Result<bool>.Failure("El stock del departamento no fue encontrado.");

            await _repository.DeleteAsync(stockDepartment);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar el stock: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetStockByDepartmentIdAsync(Guid departmentId)
    {
        var stockDepartments = await _repository.GetStockByDepartmentAsync(departmentId);
        
        var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(stockDepartments);
        
        return Result<IEnumerable<StockDepartmentDto>>.Success(response);
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetLowStockByDepartmentIdAsync(Guid departmentId)
    {
        try
        {
            var lowStockDepartments = await _repository.GetBelowMinQuantityAsync(departmentId);
            
            var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(lowStockDepartments);
            
            return Result<IEnumerable<StockDepartmentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<StockDepartmentDto>>.Failure($"Error al obtener stock bajo del departamento: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<StockDepartmentDto>>> GetOverStockByDepartmentIdAsync(Guid departmentId)
    {
        try
        {
            var overStockDepartments = await _repository.GetAboveMaxQuantityAsync(departmentId);
            
            var response = _mapper.Map<IEnumerable<StockDepartmentDto>>(overStockDepartments);
            
            return Result<IEnumerable<StockDepartmentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<StockDepartmentDto>>.Failure($"Error al obtener sobrestock del departamento: {ex.Message}");
        }
    }
}