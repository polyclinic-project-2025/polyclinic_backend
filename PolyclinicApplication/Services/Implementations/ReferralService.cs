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
        public async Task<ReferralDto> CreateAsync(CreateReferralDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            
            // Validar si no existe Puesto Externo
            var existPe = await _PeRepo.GetByIdAsync(dto.ExternalMedicalPostId);
            if (existPe is null)
                throw new InvalidOperationException($"No existe el Puesto Externo");

            // Validar si no existe DepartmentTo
            var existDT = await _departmentRepo.GetByIdAsync(dto.DepartmentToId);
            if (existDT is null)
                throw new InvalidOperationException($"No existe el Departamento de destino");
            //Validar si no existe el Paciente
            var existPac = await _patientRepo.GetByIdAsync(dto.PatientId);
            if (existPac is null)
                throw new InvalidOperationException($"No existe el Paciente asignado");
            var referral = new Referral(
                Guid.NewGuid(),
                dto.PatientId,
                dto.DateTimeRem,
                dto.ExternalMedicalPostId,
                dto.DepartmentToId
            );

            await _repo.AddAsync(referral);

            return _mapper.Map<ReferralDto>(referral);
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
    public async Task<IEnumerable<ReferralDto>> GetAllAsync()
    {
        var entities = await _repo.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<ReferralDto>>(entities);
    }

    // ----------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------
    public async Task<ReferralDto?> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdWithIncludesAsync(id);
        return entity is null ? null : _mapper.Map<ReferralDto>(entity);
    }

    // ----------------------------------------------------------
    // SEARCHES
    // ----------------------------------------------------------
    public async Task<IEnumerable<ReferralDto>> SearchByPuestoExternoAsync(string name)
    {
        var result = await _repo.GetByPuestoExternoAsync(name);
        return _mapper.Map<IEnumerable<ReferralDto>>(result);
    }

    public async Task<IEnumerable<ReferralDto>> SearchByDepartmentToNameAsync(string name)
    {
        var result = await _repo.GetByDepartmentToNameAsync(name);
        return _mapper.Map<IEnumerable<ReferralDto>>(result);
    }

    public async Task<IEnumerable<ReferralDto>> SearchByPatientNameAsync(string patientName)
    {
        var result = await _repo.GetByPatientNameAsync(patientName);
        return _mapper.Map<IEnumerable<ReferralDto>>(result);
    }

    public async Task<IEnumerable<ReferralDto>> SearchByDateAsync(DateTime date)
    {
        var result = await _repo.GetByDateAsync(date.Date);
        return _mapper.Map<IEnumerable<ReferralDto>>(result);
    }
    public async Task<IEnumerable<ReferralDto>> SearchByPatientIdentificationAsync(string patientIdentification)
    {
        var result = await _repo.GetByPatientIdentificationAsync(patientIdentification);
        return _mapper.Map<IEnumerable<ReferralDto>>(result);
    }
}
}