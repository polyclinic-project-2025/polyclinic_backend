using AutoMapper;
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

    public DerivationService(
        IDerivationRepository repo,
        IPatientRepository patientRepo,
        IDepartmentRepository departmentRepo,
        IMapper mapper)
    {
        _repo = repo;
        _patientRepo = patientRepo;
        _departmentRepo = departmentRepo;
        _mapper = mapper;
    }
    // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<Result<DerivationDto>> CreateAsync(CreateDerivationDto dto)
        {
            try
            {
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

                await _repo.AddAsync(derivation);

                var derivationdto = _mapper.Map<DerivationDto>(derivation);
                return Result<DerivationDto>.Success(derivationdto);
            }
            catch (Exception ex)
            {
                return Result<DerivationDto>.Failure($"Error al guardar la derivación");
            }
        }

    // ----------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------
    public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _repo.GetByIdAsync(id);
                if (result is null)
                    return Result<bool>.Failure("Derivacion no encontrada.");

                await _repo.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar la derivación");
            }
        }

    //READ
    // ----------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<DerivationDto>>> GetAllAsync()
    {
        try
        {
            var entities = await _repo.GetAllWithIncludesAsync();
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(entities);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al obtener las derivaciones");
        }
    }

    // ----------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------
    public async Task<Result<DerivationDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _repo.GetByIdWithIncludesAsync(id);
            if(entity == null)
                {
                    return Result<DerivationDto>.Failure("Derivacion no encontrada.");
                }
            var derivationdto = _mapper.Map<DerivationDto>(entity);
            return Result<DerivationDto>.Success(derivationdto);
        }
        catch (Exception ex)
        {
            return Result<DerivationDto>.Failure($"Error al obtener la derivación");
        }
    }

    // ----------------------------------------------------------
    // SEARCHES
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentFromNameAsync(string name)
    {
        try
        {
            var result = await _repo.GetByDepartmentFromNameAsync(name);
            if(!result.Any())
                {
                    return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
                }
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al buscar derivaciones");
        }
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDepartmentToNameAsync(string name)
    {
        try
        {
            var result = await _repo.GetByDepartmentToNameAsync(name);
            if(!result.Any())
                {
                    return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
                }
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al buscar derivaciones");
        }
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByPatientNameAsync(string patientName)
    {
        try
        {
            var result = await _repo.GetByPatientNameAsync(patientName);
            if(!result.Any())
                {
                    return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
                }
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al buscar derivaciones");
        }
    }

    public async Task<Result<IEnumerable<DerivationDto>>> SearchByDateAsync(DateTime date)
    {
        try
        {
            var result = await _repo.GetByDateAsync(date.Date);
            if(!result.Any())
                {
                    return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
                }
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al buscar derivaciones");
        }
    }
    public async Task<Result<IEnumerable<DerivationDto>>> SearchByPatientIdentificationAsync(string patientIdentification)
    {
        try
        {
            var result = await _repo.GetByPatientIdentificationAsync(patientIdentification);
            if(!result.Any())
                {
                    return Result<IEnumerable<DerivationDto>>.Failure("Derivacion no encontrada.");
                }
            var derivationsdto = _mapper.Map<IEnumerable<DerivationDto>>(result);
            return Result<IEnumerable<DerivationDto>>.Success(derivationsdto);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DerivationDto>>.Failure($"Error al buscar derivaciones");
        }
    }
}
}

