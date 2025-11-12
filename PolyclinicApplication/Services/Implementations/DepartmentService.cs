using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicDomain.Entities;

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

        // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            // Validación con FluentValidation
            await _createValidator.ValidateAndThrowAsync(dto);

            // Regla de negocio: evitar duplicados por nombre
            var exists = await _repository.ExistsByNameAsync(dto.Name);
            if (exists)
                throw new InvalidOperationException($"A department with the name '{dto.Name}' already exists.");

            // Crear entidad de dominio
            var department = new Department(Guid.NewGuid(), dto.Name, dto.HeadId);

            // Persistir
            await _repository.AddAsync(department);

            // Retornar mapeo a DTO
            return _mapper.Map<DepartmentDto>(department);
        }

        // -------------------------------
        // READ
        // -------------------------------
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(entities);
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var department = await _repository.GetWithHeadAsync(id);
            return _mapper.Map<DepartmentDto?>(department);
        }

        // -------------------------------
        // UPDATE
        // -------------------------------
        public async Task UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var department = await _repository.GetByIdAsync(id);
            if (department == null)
                throw new KeyNotFoundException("Department not found.");

            // Evitar duplicado si cambia el nombre
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != department.Name)
            {
                var exists = await _repository.ExistsByNameAsync(dto.Name);
                if (exists)
                    throw new InvalidOperationException($"A department with the name '{dto.Name}' already exists.");

                department.ChangeName(dto.Name);
            }

            // Procesar HeadId
            if (dto.HeadId.HasValue)
            {
                department.AssignHead(dto.HeadId.Value);
            }
            else if (dto.HeadId == null)
            {
                department.RemoveHead();
            }

            await _repository.UpdateAsync(department);
        }

        // -------------------------------
        // DELETE
        // -------------------------------
        public async Task DeleteAsync(Guid id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException("Department not found.");

            await _repository.DeleteByIdAsync(id);
        }
    }
}
