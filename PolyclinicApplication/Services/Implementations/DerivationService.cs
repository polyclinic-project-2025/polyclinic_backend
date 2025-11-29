using AutoMapper;
using FluentValidation;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.Services.Implementations{

public class DerivationService : IDerivationService
{
    private readonly IDerivationRepository _repo;
    private readonly IPatientRepository _patientRepo;
    private readonly IDepartmentRepository _departmentRepo;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDerivationDto> _createValidator;

    public DerivationService(
        IDerivationRepository repo,
        IPatientRepository patientRepo,
        IDepartmentRepository departmentRepo,
        IMapper mapper,
        IValidator<CreateDerivationDto> createValidator)
    {
        _repo = repo;
        _patientRepo = patientRepo;
        _departmentRepo = departmentRepo;
        _mapper = mapper;
        _createValidator = createValidator;
    }
    // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<DerivationDto> CreateAsync(CreateDerivationDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            
            // Validar si no existe DepartmentFrom
            var existDF = await _departmentRepo.GetByIdAsync(dto.DepartmentFromId);
            if (existDF is null)
                throw new InvalidOperationException($"No existe el Departamento de origen");

            // Validar si no existe DepartmentTo
            var existDT = await _departmentRepo.GetByIdAsync(dto.DepartmentToId);
            if (existDT is null)
                throw new InvalidOperationException($"No existe el Departamento de destino");
            //Validar si no existe el Paciente
            var existPac = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (existPac is null)
                throw new InvalidOperationException($"No existe el Paciente asignado");
            var derivation = new Derivation(
                Guid.NewGuid(),
                dto.DepartmentFromId,
                dto.DateTimeDer,
                dto.PatientId,
                dto.DepartmentToId
            );

            await _repo.AddAsync(derivation);

            return _mapper.Map<DerivationDto>(derivation);
        }

    // ----------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------
    public async Task DeleteAsync(Guid id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result is null)
                throw new KeyNotFoundException("Paciente no encontrado.");

            await _repo.DeleteByIdAsync(id);
        }

    //READ
    // ----------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------
    public async Task<IEnumerable<DerivationDto>> GetAllAsync()
    {
        var entities = await _repo.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<DerivationDto>>(entities);
    }

    // ----------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------
    public async Task<DerivationDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdWithIncludesAsync(id);
        return entity is null ? null : _mapper.Map<DerivationDto>(entity);
    }

    // ----------------------------------------------------------
    // SEARCHES
    // ----------------------------------------------------------
    public async Task<IEnumerable<DerivationDto>> SearchByDepartmentFromNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentFromNameAsync(name);
        return _mapper.Map<IEnumerable<DerivationDto>>(result);
    }

    public async Task<IEnumerable<DerivationDto>> SearchByDepartmentToNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentToNameAsync(name);
        return _mapper.Map<IEnumerable<DerivationDto>>(result);
    }

    public async Task<IEnumerable<DerivationDto>> SearchByPatientNameAsync(string patientName)
    {
        var result = await _repo.GetByPatientNameAsync(patientName);
        return _mapper.Map<IEnumerable<DerivationDto>>(result);
    }

    public async Task<IEnumerable<DerivationDto>> SearchByDateAsync(DateTime date)
    {
        var result = await _repo.GetByDateAsync(date.Date);
        return _mapper.Map<IEnumerable<DerivationDto>>(result);
    }
    public async Task<IEnumerable<DerivationDto>> SearchByPatientIdentificationAsync(string patientIdentification)
    {
        var result = await _repo.GetByPatientIdentificationAsync(patientIdentification);
        return _mapper.Map<IEnumerable<DerivationDto>>(result);
    }
}
}

