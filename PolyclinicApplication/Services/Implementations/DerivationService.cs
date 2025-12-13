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
using PolyclinicApplication.Common.Results;

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
        public async Task<Result<DerivationDto>> CreateAsync(CreateDerivationDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);
            if(!result.IsValid)
            {
                return Result<DerivationDto>.Failure(result.Errors.First().ErrorMessage);
            }
            // Validar si no existe DepartmentFrom
            var existDF = await _departmentRepo.GetByIdAsync(dto.DepartmentFromId);
            if (existDF is null)
                return Result<DerivationDto>.Failure("Departamento de origen no encontrado.");

            // Validar si no existe DepartmentTo
            var existDT = await _departmentRepo.GetByIdAsync(dto.DepartmentToId);
            if (existDT is null)
                return Result<DerivationDto>.Failure("Departamento de destino no encontrado.");
            //Validar si no existe el Paciente
            var existPac = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (existPac is null)
                return Result<DerivationDto>.Failure("Paciente no encontrado.");
            var derivation = new Derivation(
                Guid.NewGuid(),
                dto.DepartmentFromId,
                dto.DateTimeDer,
                dto.PatientId,
                dto.DepartmentToId
            );

            try
            {
                await _repo.AddAsync(derivation);
            }
            catch (Exception ex)
            {
                return Result<DerivationDto>.Failure($"Error al guardar la derivación: {ex.Message}");
            }

            var derivationdto = _mapper.Map<DerivationDto>(derivation);
            return Result<DerivationDto>.Success(derivationdto);
        }

    // ----------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------
    public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result is null)
                return Result<bool>.Failure("Derivacion no encontrada.");

            try
            {
                await _repo.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar la derivación: {ex.Message}");
            }
        }

    //READ
    // ----------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<DerivationDto>>> GetAllAsync()
    {
        var entities = await _repo.GetAllWithIncludesAsync();
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(entities);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }

    // ----------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------
    public async Task<Result<DerivationDto>> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdWithIncludesAsync(id);
        if(entity == null)
            {
                return Result<DerivationDto>.Failure("Derivacion no encontrada.");
            }
        var derivationdto = _mapper.Map<DerivationDto>(entity);
        return Result<DerivationDto>.Success(derivationdto);
    }

    // ----------------------------------------------------------
    // SEARCHES
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentFromNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentFromNameAsync(name);
        if(!result.Any())
            {
                return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
            }
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentToNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentToNameAsync(name);
        if(!result.Any())
            {
                return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
            }
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByPatientNameAsync(string patientName)
    {
        var result = await _repo.GetByPatientNameAsync(patientName);
        if(!result.Any())
            {
                return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
            }
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDateAsync(DateTime date)
    {
        var result = await _repo.GetByDateAsync(date.Date);
        if(!result.Any())
            {
                return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
            }
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }
    public async Task<Result<IEnumerable<DerivationDto>>> SearchByPatientIdentificationAsync(string patientIdentification)
    {
        var result = await _repo.GetByPatientIdentificationAsync(patientIdentification);
        if(!result.Any())
            {
                return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
            }
        var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
        return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
    }
}
}

