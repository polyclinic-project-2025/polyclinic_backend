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
    private readonly IDepartmentHeadRepository _departmentHeadRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    public ConsultationReferralService(IConsultationReferralRepository consultationReferralRepository
    ,IReferralRepository referralRepository, IDepartmentRepository departmentRepository, IDepartmentHeadRepository departmentHeadRepository
    ,IDoctorRepository doctorRepository, IPatientRepository patientRepository)
    {
        _consultationReferralRepository = consultationReferralRepository;
        _referralRepository = referralRepository;
        _departmentRepository = departmentRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
    }
    public async Task<Result<ConsultationReferralResponse>> CreateAsync(CreateConsultationReferralDto request)
    {
        //validaciones 
        //1. jefe de dpt exista
        var head = await _departmentHeadRepository.GetByIdAsync(request.DepartmentHeadId);
        if(head == null) return Result<ConsultationReferralResponse>.Failure("El Jefe de Departamento especificado no existe");
        
        //2. paciente en remision exista
        var patientReferral = await _referralRepository.GetByIdAsync(request.ReferralId);
        if(patientReferral == null) return Result<ConsultationReferralResponse>.Failure("El paciente especificado no existe en remisión");
        
        //3. fecha no sea futura 
        var dateTimeNow = DateTime.Now;
        if (dateTimeNow < request.DateTimeCRem) return Result<ConsultationReferralResponse>.Failure("La fecha de consulta no es válida");
    
        //4. doctor tratante en existencia y perteneciente al mismo dpt que el jefe de dpt

        var departmentId = head.DepartmentId;
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        if(doctor == null || doctor.DepartmentId != departmentId) return Result<ConsultationReferralResponse>.Failure("El doctor tratante especificado no existe en el departamento consultante");
        
        if(request.Diagnosis is null || request.Diagnosis == "") return Result<ConsultationReferralResponse>.Failure("Debe proporcionar un diagnóstico");
        
        var departmentName = head.Department!.Name;
        var patient = patientReferral.Patient;
        return Result<ConsultationReferralResponse>.Success(new ConsultationReferralResponse
        {
            ReferralId = request.ReferralId,
            DepartmentHeadId = request.DepartmentHeadId,
            DoctorId = request.DoctorId,
            DateTimeCRem = request.DateTimeCRem,
            Diagnosis = request.Diagnosis,
            DepartmentName = departmentName,
            DoctorFullName = doctor.Name,
            PatientFullName = patient!.Name,

        });
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var result = _consultationReferralRepository.DeleteByIdAsync(id);
        if(result.IsCompleted) return Result<bool>.Success(true);
        return Result<bool>.Failure("Error al eliminar consulta");
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetAllAsync()
    {
        var result = await _consultationReferralRepository.GetAllAsync();
        List<ConsultationReferralResponse> list = new ();
        foreach (var item in result)
        {
            list.Add(
            new ConsultationReferralResponse{
            ReferralId = item.ReferralId,
            DepartmentHeadId = item.DepartmentHeadId,
            DoctorId = item.DoctorId,
            DateTimeCRem = item.DateTimeCRem,
            Diagnosis = item.Diagnosis,
            DepartmentName = item.DepartmentHead!.Department!.Name,
            DoctorFullName = item.Doctor!.Name,
            PatientFullName = item.Referral!.Patient!.Name,
        });}
    
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(list);
    }

    public async Task<Result<ConsultationReferralResponse>> GetByIdAsync(Guid id)
    {
        var result = await _consultationReferralRepository.GetByIdAsync(id);
        if(result == null) return Result<ConsultationReferralResponse>.Failure($"Consulta con id: {id} no encontrada");
        return Result<ConsultationReferralResponse>.Success(
            new ConsultationReferralResponse
            {
                ReferralId = result.ReferralId,
                DepartmentHeadId = result.DepartmentHeadId,
                DoctorId = result.DoctorId,
                DateTimeCRem = result.DateTimeCRem,
                Diagnosis = result.Diagnosis,
                DepartmentName = result.DepartmentHead!.Department!.Name,
                DoctorFullName = result.Doctor!.Name,
                PatientFullName = result.Referral!.Patient!.Name,
            }
        );
    }

    public async Task<Result<ConsultationReferralResponse>> UpdateAsync(Guid id, UpdateConsultationReferralDto request)
    {
        var consultation = await _consultationReferralRepository.GetByIdAsync(id);
        if(consultation == null) return Result<ConsultationReferralResponse>.Failure($"Consulta {id} no encontrrada");
        
        var referralId = request.ReferralId == null ? consultation.ReferralId : request.ReferralId;
        var departmentHeadId = request.DepartmentHeadId == null ? consultation.DepartmentHeadId : request.DepartmentHeadId;
        var doctorId = request.DoctorId == null ? consultation.DoctorId : request.DoctorId;
        var dateTimeCRem = request.DateTimeCRem == null ? consultation.DateTimeCRem : request.DateTimeCRem;
        
        var dateTimeNow = DateTime.Now;
        if (dateTimeNow < request.DateTimeCRem) return Result<ConsultationReferralResponse>.Failure("La fecha de consulta no es válida");
        var diagnosis = request.Diagnosis == null ? consultation.Diagnosis : request.Diagnosis;
        
        var head = await _departmentHeadRepository.GetByIdAsync(departmentHeadId.Value);
        if(head == null) return Result<ConsultationReferralResponse>.Failure("El Jefe de Departamento especificado no existe");
        var departmentName = head!.Department!.Name;
        var doctor = await _doctorRepository.GetByIdAsync(doctorId.Value);
        if(doctor!.DepartmentId != head.DepartmentId) return Result<ConsultationReferralResponse>.Failure("El doctor tratante especificado no existe en el departamento consultante");
        
        var doctorName = doctor!.Name;
        var patient = await _referralRepository.GetByIdAsync(referralId.Value);
        if(patient == null) return Result<ConsultationReferralResponse>.Failure("El paciente especificado no existe en remisión");
        var patientFullName = patient!.Patient!.Name;

        return Result<ConsultationReferralResponse>.Success(
            new ConsultationReferralResponse
            {
                ReferralId = referralId.Value,
                DepartmentHeadId = departmentHeadId.Value,
                DoctorId = doctorId.Value,
                DateTimeCRem = dateTimeCRem.Value,
                Diagnosis = diagnosis,
                DepartmentName = departmentName,
                DoctorFullName = doctorName,
                PatientFullName = patientFullName,
            }
        );
    }
}