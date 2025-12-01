using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicApplication.Common.Results;
using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;

namespace PolyclinicApplication.Services.Implementations;

public class ConsultationReferralService : IConsultationReferralService
{
    private readonly IConsultationReferralRepository _consultationReferralRepository;
    private readonly IReferralRepository _referralRepository;
    private readonly IDepartmentRepository _departmentRepository;
    // private readonly IDepartmentHeadRepository _departmentHeadRepository;
    public ConsultationReferralService(IConsultationReferralRepository consultationReferralRepository
    ,IReferralRepository referralRepository, IDepartmentRepository departmentRepository)
    {
        _consultationReferralRepository = consultationReferralRepository;
        _referralRepository = referralRepository;
        _departmentRepository = departmentRepository;
    }
    public Task<Result<ConsultationReferralResponse>> CreateAsync(ConsultationReferralDto request)
    {
        //validaciones 
        //1. jefe de dpt exista
        //2. paciente en remision exista
        //3. fecha no sea futura 
        //4. doctor tratante en existencia y perteneciente al mismo dpt que el jefe de dpt
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetAllAsync()
    {
        var result = await _consultationReferralRepository.GetAllAsync();
        //creaer los response 
        //hay que buscar nombre de dpt etc
        // return Result<IEnumerable<ConsultationReferralResponse>>.Success(result);
        throw new NotImplementedException();
    }

    public Task<Result<ConsultationReferralResponse>> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ConsultationReferralResponse>> UpdateAsync(ConsultationReferralDto request)
    {
        throw new NotImplementedException();
    }
}