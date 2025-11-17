using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class WarehouseManagerService : IWarehouseManagerService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public WarehouseManagerService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<WarehouseManagerResponseDto>> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as WarehouseManager;
            if (employee == null)
                return Result<WarehouseManagerResponseDto>.Failure("Jefe de almacén no encontrado.");

            return Result<WarehouseManagerResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<IEnumerable<WarehouseManagerResponseDto>>> GetAllAsync()
        {
            var employees = (await _employeeRepository.GetAllAsync())
                            .OfType<WarehouseManager>();
            var managersList = employees.Select(MapToResponse);
            return Result<IEnumerable<WarehouseManagerResponseDto>>.Success(managersList);
        }

        public async Task<Result<WarehouseManagerResponseDto>> CreateAsync(WarehouseManagerDto dto)
        {
            var manager = new WarehouseManager(
                Guid.NewGuid(),
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.ManagedWarehouseId
            );

            await _employeeRepository.AddAsync(manager);

            return Result<WarehouseManagerResponseDto>.Success(MapToResponse(manager));
        }

        public async Task<Result<WarehouseManagerResponseDto>> UpdateAsync(Guid id, WarehouseManagerDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as WarehouseManager;
            if (employee == null)
                return Result<WarehouseManagerResponseDto>.Failure("Jefe de almacén no encontrado.");

            // Crear nueva instancia actualizada si las propiedades son solo lectura
            var updatedEntity = new WarehouseManager(
                employee.Id,
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                employee.ManagedWarehouseId
            );

            await _employeeRepository.UpdateAsync(updatedEntity);

            return Result<WarehouseManagerResponseDto>.Success(MapToResponse(updatedEntity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as WarehouseManager;
            if (employee == null)
                return Result<bool>.Failure("Encargado de almacén no encontrado");

            await _employeeRepository.DeleteAsync(employee);

            return Result<bool>.Success(true);
        }

        private WarehouseManagerResponseDto MapToResponse(WarehouseManager manager)
        {
            return new WarehouseManagerResponseDto(
                Id: manager.Id,                              
                Identification: manager.Identification,
                Name: manager.Name,                    
                EmploymentStatus: manager.EmploymentStatus,
                PrimaryRole: manager.GetPrimaryRole(),        
                ManagedWarehouseId: manager.ManagedWarehouseId
            );
        }
    }
}
