using AutoMapper;
using FluentValidation;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.DTOs.Request.Referral;
using PolyclinicApplication.DTOs.Response.Referral;
using PolyclinicApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Implementations{

public class ReferralService : IReferralService
{
    private readonly IReferralRepository _repo;
    private readonly IPatientRepository _patientRepo;
    private readonly IPuestoExternoRepository _PeRepo;
    private readonly IDepartmentRepository _departmentRepo;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateReferralDto> _createValidator;

    public ReferralService(
        IReferralRepository repo,
        IPatientRepository patientRepo,
        IPuestoExternoRepository PeRepo,
        IDepartmentRepository departmentRepo,
        IMapper mapper,
        IValidator<CreateReferralDto> createValidator)
    {
        _repo = repo;
        _patientRepo = patientRepo;
        _PeRepo = PeRepo;
        _departmentRepo = departmentRepo;
        _mapper = mapper;
        _createValidator = createValidator;
    }
    // -------------------------------
        // CREATE
        // -------------------------------
        public async Task<Result<ReferralDto>> CreateAsync(CreateReferralDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);
            if(!result.IsValid)
            {
                return Result<ReferralDto>.Failure(result.Errors.First().ErrorMessage);
            }
            
            // Validar si no existe Puesto Externo
            var existPe = await _PeRepo.GetByNameAsync(dto.PuestoExterno);
            if (existPe is null)
            {
                try
                {
                    var pe = await _PeRepo.AddAsync(new ExternalMedicalPost(Guid.NewGuid(),dto.PuestoExterno));
                    existPe = pe;
                }
                catch (Exception ex)
                {
                    return Result<ReferralDto>.Failure($"Error al guardar el puesto externo: {ex.Message}");
                }
            }   
                

            // Validar si no existe DepartmentTo
            var existDT = await _departmentRepo.GetByIdAsync(dto.DepartmentToId);
            if (existDT is null)
                return Result<ReferralDto>.Failure("Departamento de destino no encontrado.");
            //Validar si no existe el Paciente
            var existPac = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (existPac is null)
                return Result<ReferralDto>.Failure("Paciente no encontrado.");
            var referral = new Referral(
                Guid.NewGuid(),
                dto.PatientId,
                dto.DateTimeRem,
                existPe.ExternalMedicalPostId,
                dto.DepartmentToId
            );

            try
            {
                await _repo.AddAsync(referral);
            }
            catch (Exception ex)
            {
                return Result<ReferralDto>.Failure($"Error al guardar la remisión: {ex.Message}");
            }

            var referraldto = _mapper.Map<ReferralDto>(referral);
            return Result<ReferralDto>.Success(referraldto);
        }

    // ----------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------
    public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result is null)
                return Result<bool>.Failure("Remision no encontrada.");

            try
            {
                await _repo.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar la remisión: {ex.Message}");
            }
        }

    //READ
    // ----------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<ReferralDto>>> GetAllAsync()
    {
        var entities = await _repo.GetAllWithIncludesAsync();
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(entities);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }

    // ----------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------
    public async Task<Result<ReferralDto>> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdWithIncludesAsync(id);
        if(entity == null)
            {
                return Result<ReferralDto>.Failure("Remision no encontrada.");
            }
        var referraldto = _mapper.Map<ReferralDto>(entity);
        return Result<ReferralDto>.Success(referraldto);
    }

    // ----------------------------------------------------------
    // SEARCHES
    // ----------------------------------------------------------
    public async Task<Result<IEnumerable<ReferralDto>>> SearchByPuestoExternoAsync(string name)
    {
        var result = await _repo.GetByPuestoExternoAsync(name);
        if(!result.Any())
            {
                return Result<IEnumerable<ReferralDto>>.Failure("Remision no encontrada.");
            }
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(result);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }

    public async Task<Result<IEnumerable<ReferralDto>>> SearchByDepartmentToNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentToNameAsync(name);
        if(!result.Any())
            {
                return Result<IEnumerable<ReferralDto>>.Failure("Remision no encontrada.");
            }
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(result);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }

    public async Task<Result<IEnumerable<ReferralDto>>> SearchByPatientNameAsync(string patientName)
    {
        var result = await _repo.GetByPatientNameAsync(patientName);
        if(!result.Any())
            {
                return Result<IEnumerable<ReferralDto>>.Failure("Remision no encontrada.");
            }
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(result);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }

    public async Task<Result<IEnumerable<ReferralDto>>> SearchByDateAsync(DateTime date)
    {
        var result = await _repo.GetByDateAsync(date.Date);
        if(!result.Any())
            {
                return Result<IEnumerable<ReferralDto>>.Failure("Remision no encontrada.");
            }
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(result);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }
    public async Task<Result<IEnumerable<ReferralDto>>> SearchByPatientIdentificationAsync(string patientIdentification)
    {
        var result = await _repo.GetByPatientIdentificationAsync(patientIdentification);
        if(!result.Any())
            {
                return Result<IEnumerable<ReferralDto>>.Failure("Remision no encontrada.");
            }
        var referralsdto = _mapper.Map<IEnumerable<ReferralDto>>(result);
        return Result<IEnumerable<ReferralDto>>.Success(referralsdto);
    }
}
}