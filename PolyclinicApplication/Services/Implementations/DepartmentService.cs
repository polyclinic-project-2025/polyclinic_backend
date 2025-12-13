using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicDomain.Entities;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateDepartmentDto> _createValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateValidator;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper,
            IValidator<CreateDepartmentDto> createValidator,
            IValidator<UpdateDepartmentDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }


        public async Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
        {
            // Validación con FluentValidation
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Result<DepartmentDto>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            // Verificar si ya existe un departamento con ese nombre
            var exists = await _repository.ExistsByNameAsync(dto.Name);
            if (exists)
                return Result<DepartmentDto>.Failure($"Ya existe un departamento con el nombre '{dto.Name}'.");

            // Crear entidad
            var department = new Department(Guid.NewGuid(), dto.Name);

            // Guardar en BD con manejo de errores
            try
            {
                await _repository.AddAsync(department);
            }
            catch (DbUpdateException ex)
            {
                // Detectar errores comunes por el mensaje de la excepción
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                if (errorMessage.Contains("duplicate key") || errorMessage.Contains("unique constraint"))
                    return Result<DepartmentDto>.Failure("Ya existe un departamento con ese nombre.");
                if (errorMessage.Contains("foreign key"))
                    return Result<DepartmentDto>.Failure("Error de integridad referencial.");
                if (errorMessage.Contains("null value"))
                    return Result<DepartmentDto>.Failure("Falta un campo obligatorio.");
                
                return Result<DepartmentDto>.Failure($"Error al guardar en la base de datos: {errorMessage}");
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure($"Error inesperado: {ex.Message}");
            }

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return Result<DepartmentDto>.Success(departmentDto);
        }

        public async Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                var entitiesDtos = _mapper.Map<IEnumerable<DepartmentDto>>(entities);
                return Result<IEnumerable<DepartmentDto>>.Success(entitiesDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DepartmentDto>>.Failure($"Error al obtener departamentos: {ex.Message}");
            }
        }

        public async Task<Result<DepartmentDto?>> GetByIdAsync(Guid id)
        {
            try
            {
                var department = await _repository.GetWithHeadAsync(id);
                if (department == null)
                    return Result<DepartmentDto?>.Failure("Departamento no encontrado.");

                var departmentDto = _mapper.Map<DepartmentDto>(department);
                return Result<DepartmentDto?>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto?>.Failure($"Error al obtener departamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            // Validación con FluentValidation
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return Result<bool>.Failure(validationResult.Errors.First().ErrorMessage);
            }

            try
            {
                // Obtener departamento
                var department = await _repository.GetByIdAsync(id);
                if (department == null)
                    return Result<bool>.Failure("Departamento no encontrado.");

                // Actualizar nombre si cambió
                if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != department.Name)
                {
                    var exists = await _repository.ExistsByNameAsync(dto.Name);
                    if (exists)
                        return Result<bool>.Failure($"Ya existe un departamento con el nombre '{dto.Name}'.");

                    department.ChangeName(dto.Name);
                }

                // Guardar cambios
                await _repository.UpdateAsync(department);
                return Result<bool>.Success(true);
            }
            catch (DbUpdateException ex)
            {
                return Result<bool>.Failure($"Error al actualizar departamento: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error inesperado: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                // Verificar que existe
                var department = await _repository.GetByIdAsync(id);
                if (department == null)
                    return Result<bool>.Failure("Departamento no encontrado.");

                // Eliminar
                await _repository.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (DbUpdateException ex)
            {
                return Result<bool>.Failure($"Error al eliminar departamento: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error inesperado: {ex.Message}");
            }
        }


        public async Task<Result<List<Doctor>>> GetDoctorsByDepartmentIdAsync(Guid departmentId)
        {
            try
            {
                var doctors = await _repository.GetDoctorsByDepartmentIdAsync(departmentId);
                return Result<List<Doctor>>.Success(doctors);
            }
            catch (Exception ex)
            {
                return Result<List<Doctor>>.Failure($"Error al obtener doctores del departamento: {ex.Message}");
            }
        }
    }
}
