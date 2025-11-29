using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;

namespace PolyclinicApplication.Services.Implementations
{
    public class PuestoExternoService : IPuestoExternoService
    {
        private readonly IPuestoExternoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePuestoExternoDto> _createValidator;

        public PuestoExternoService(
            IPuestoExternoRepository repository,
            IMapper mapper,
            IValidator<CreatePuestoExternoDto> createValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<PuestoExternoDto> CreateAsync(CreatePuestoExternoDto dto)
        {
            // Validación con FluentValidation
            await _createValidator.ValidateAndThrowAsync(dto);

            // Regla de negocio: evitar duplicados por nombre
            var exists = await _repository.GetByNameAsync(dto.Name);
            if (exists != null)
                throw new InvalidOperationException($"A ExternalMedicalPost with the name '{dto.Name}' already exists.");

            // Crear entidad de dominio
            var pe = new ExternalMedicalPost(Guid.NewGuid(), dto.Name,dto.Address);

            // Persistir
            await _repository.AddAsync(pe);

            // Retornar mapeo a DTO
            return _mapper.Map<PuestoExternoDto>(pe);
        }

        // -------------------------------
        // READ
        // -------------------------------
        public async Task<IEnumerable<PuestoExternoDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PuestoExternoDto>>(entities);
        }

        public async Task<PuestoExternoDto?> GetByIdAsync(Guid id)
        {
            var pe = await _repository.GetByIdAsync(id);
            return _mapper.Map<PuestoExternoDto?>(pe);
        }

        // -------------------------------
        // DELETE
        // -------------------------------
        public async Task DeleteAsync(Guid id)
        {
            var exists = await _repository.GetByIdAsync(id);
            if (exists == null)
                throw new KeyNotFoundException("EXternalMedicalPost not found.");

            await _repository.DeleteByIdAsync(id);
        }
    }
}
