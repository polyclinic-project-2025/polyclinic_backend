using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class DepartmentHeadService : IDepartmentHeadService
    {
        private readonly IDepartmentHeadRepository _repository;
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentHeadService(
            IDepartmentHeadRepository repository,
            IRepository<Department> departmentRepository)
        {
            _repository = repository; 
            _departmentRepository = departmentRepository;
        }

        public async Task<Result<DepartmentHeadResponseDto>> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return Result<DepartmentHeadResponseDto>.Failure("Jefe de departamento no encontrado");

            // Obtener el departamento si existe
            Department? department = null;
            if (entity.ManagedDepartmentId.HasValue)
            {
                department = await _departmentRepository.GetByIdAsync(entity.ManagedDepartmentId.Value);
            }

            var response = new DepartmentHeadResponseDto
            {
                Id = entity.Id,
                Identification = entity.Identification,
                Name = entity.Name ?? string.Empty,
                EmploymentStatus = entity.EmploymentStatus ?? string.Empty,
                PrimaryRole = entity.GetPrimaryRole(),
                ManagedDepartmentId = entity.ManagedDepartmentId,
                ManagedDepartmentName = department?.Name,
                UserId = entity.UserId
            };

            return Result<DepartmentHeadResponseDto>.Success(response);
        }

        public async Task<Result<IEnumerable<DepartmentHeadResponseDto>>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var responseTasks = entities.Select(async e =>
            {
                Department? department = null;
                if (e.ManagedDepartmentId.HasValue)
                {
                    department = await _departmentRepository.GetByIdAsync(e.ManagedDepartmentId.Value);
                }

                return new DepartmentHeadResponseDto
                {
                    Id = e.Id,
                    Identification = e.Identification,
                    Name = e.Name ?? string.Empty,
                    EmploymentStatus = e.EmploymentStatus ?? string.Empty,
                    PrimaryRole = e.GetPrimaryRole(),
                    ManagedDepartmentId = e.ManagedDepartmentId,
                    ManagedDepartmentName = department?.Name,
                    UserId = e.UserId
                };
            });

            var list = await Task.WhenAll(responseTasks);
            return Result<IEnumerable<DepartmentHeadResponseDto>>.Success(list);
        }

        public async Task<Result<DepartmentHeadResponseDto>> CreateAsync(DepartmentHeadDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Identification))
                return Result<DepartmentHeadResponseDto>.Failure("El número de identificación es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<DepartmentHeadResponseDto>.Failure("El nombre es obligatorio");

            // Verificar que el departamento existe
            var department = await _departmentRepository.GetByIdAsync(dto.ManagedDepartmentId);
            if (department == null)
                return Result<DepartmentHeadResponseDto>.Failure($"No existe el departamento con ID {dto.ManagedDepartmentId}");

            // Verificar que no exista otro jefe con la misma identificación
            var existingHeads = await _repository.FindAsync(h => h.Identification == dto.Identification);
            if (existingHeads.Any())
                return Result<DepartmentHeadResponseDto>.Failure("Ya existe un jefe de departamento con ese número de identificación");

            // Crear la entidad con TODOS los datos
            var entity = new DepartmentHead(
                id: Guid.NewGuid(),
                name: dto.Name,
                employmentStatus: dto.EmploymentStatus,
                identification: dto.Identification,
                managedDepartmentId: dto.ManagedDepartmentId
            );

            // Guardar en base de datos
            await _repository.AddAsync(entity);

            // Retornar respuesta con datos completos
            var response = new DepartmentHeadResponseDto
            {
                Id = entity.Id,
                Identification = entity.Identification,
                Name = entity.Name ?? string.Empty,
                EmploymentStatus = entity.EmploymentStatus ?? string.Empty,
                PrimaryRole = entity.GetPrimaryRole(),
                ManagedDepartmentId = entity.ManagedDepartmentId,
                ManagedDepartmentName = department.Name,
                UserId = entity.UserId
            };

            return Result<DepartmentHeadResponseDto>.Success(response);
        }

        public async Task<Result<DepartmentHeadResponseDto>> UpdateAsync(Guid id, DepartmentHeadDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return Result<DepartmentHeadResponseDto>.Failure("Jefe de departamento no encontrado");

            // Verificar que el departamento existe
            var department = await _departmentRepository.GetByIdAsync(dto.ManagedDepartmentId);
            if (department == null)
                return Result<DepartmentHeadResponseDto>.Failure($"No existe el departamento con ID {dto.ManagedDepartmentId}");

            // Actualizar el departamento asignado
            entity.AssignDepartment(dto.ManagedDepartmentId);

            await _repository.UpdateAsync(entity);
            // await _repository.SaveChangesAsync();

            var response = new DepartmentHeadResponseDto
            {
                Id = entity.Id,
                Identification = entity.Identification,
                Name = entity.Name ?? string.Empty,
                EmploymentStatus = entity.EmploymentStatus ?? string.Empty,
                PrimaryRole = entity.GetPrimaryRole(),
                ManagedDepartmentId = entity.ManagedDepartmentId,
                ManagedDepartmentName = department.Name,
                UserId = entity.UserId
            };

            return Result<DepartmentHeadResponseDto>.Success(response);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return Result<bool>.Failure("Jefe de departamento no encontrado");

            await _repository.DeleteAsync(entity);
            // await _repository.SaveChangesAsync();
            
            return Result<bool>.Success(true);
        }
    }
}