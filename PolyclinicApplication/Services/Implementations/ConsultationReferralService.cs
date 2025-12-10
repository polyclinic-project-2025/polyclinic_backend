using AutoMapper;
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
    private readonly IDepartmentHeadRepository _departmentHeadRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public ConsultationReferralService(
        IConsultationReferralRepository consultationReferralRepository,
        IReferralRepository referralRepository,
        IDepartmentHeadRepository departmentHeadRepository,
        IDoctorRepository doctorRepository,
        IMapper mapper)
    {
        _consultationReferralRepository = consultationReferralRepository;
        _referralRepository = referralRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<Result<ConsultationReferralResponse>> CreateAsync(CreateConsultationReferralDto request)
    {
        // 1. Validar que el jefe de departamento exista
        var head = await _departmentHeadRepository.GetByIdAsync(request.DepartmentHeadId);
        if (head == null)
            return Result<ConsultationReferralResponse>.Failure("El Jefe de Departamento especificado no existe");
        
        // 2. Validar que el paciente en remisión exista
        var patientReferral = await _referralRepository.GetByIdAsync(request.ReferralId);
        if (patientReferral == null)
            return Result<ConsultationReferralResponse>.Failure("El paciente especificado no existe en remisión");
        
        // 3. Validar que la fecha no sea futura
        if (DateTime.Now < request.DateTimeCRem)
            return Result<ConsultationReferralResponse>.Failure("La fecha de consulta no es válida");
    
        // 4. Validar que el doctor tratante exista y pertenezca al mismo departamento que el jefe
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        if (doctor == null || doctor.DepartmentId != head.DepartmentId)
            return Result<ConsultationReferralResponse>.Failure("El doctor tratante especificado no existe en el departamento consultante");
        
        // Crear la entidad usando AutoMapper
        var consultation = _mapper.Map<ConsultationReferral>(request);
        
        // Guardar en la base de datos
        consultation = await _consultationReferralRepository.AddAsync(consultation);
        Console.WriteLine($"{consultation.Doctor!.Department == null} : department null"); 
        Console.WriteLine($"{consultation.Referral!.Patient == null} : Patient null");        

        // Mapear a Response usando AutoMapper
        var response = _mapper.Map<ConsultationReferralResponse>(consultation);
        
        return Result<ConsultationReferralResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var consultation = await _consultationReferralRepository.GetByIdAsync(id);
        if (consultation == null)
            return Result<bool>.Failure("Consulta no encontrada");
        
        await _consultationReferralRepository.DeleteByIdAsync(id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetAllAsync()
    {
        var consultations = await _consultationReferralRepository.GetAllAsync();
        
        // Mapear toda la colección usando AutoMapper
        var response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(consultations);
        
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(response);
    }

    public async Task<Result<ConsultationReferralResponse>> GetByIdAsync(Guid id)
    {
        var consultation = await _consultationReferralRepository.GetByIdAsync(id);
        if (consultation == null)
            return Result<ConsultationReferralResponse>.Failure($"Consulta con id: {id} no encontrada");
        
        // Mapear a Response usando AutoMapper
        var response = _mapper.Map<ConsultationReferralResponse>(consultation);
        
        return Result<ConsultationReferralResponse>.Success(response);
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetConsultationInRange(DateTime start, DateTime end)
    {
        var consultations = await _consultationReferralRepository.GetAllAsync();
        var inRange = consultations.Where(c => c.DateTimeCRem >= start && c.DateTimeCRem <= end);
        var response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(inRange);
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetLastTen()
    {
        var consultations = await _consultationReferralRepository.GetAllAsync();
        var orderedConsultations = consultations.OrderBy(c => c.DateTimeCRem);
        List<ConsultationReferralResponse> response = new List<ConsultationReferralResponse>();
        if(orderedConsultations.Count() <= 10 )
        {
            response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(orderedConsultations).ToList();
        }
        else
        {
            var lastTen = new List<ConsultationReferral>();
            foreach(var consultation in orderedConsultations)
            {
                lastTen.Add(consultation);
            }

            response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(lastTen).ToList();
        }
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(response);
    }

    public async Task<Result<ConsultationReferralResponse>> UpdateAsync(Guid id, UpdateConsultationReferralDto request)
{
    // 1. Buscar la consulta original (ESTA instancia ya está trackeada)
    var consultation = await _consultationReferralRepository.GetByIdAsync(id);
    if (consultation == null)
        return Result<ConsultationReferralResponse>.Failure($"Consulta {id} no encontrada");

    // 2. Obtener nuevos valores o mantener los existentes
    var referralId = request.ReferralId ?? consultation.ReferralId;
    var departmentHeadId = request.DepartmentHeadId ?? consultation.DepartmentHeadId;
    var doctorId = request.DoctorId ?? consultation.DoctorId;
    var dateTimeCRem = request.DateTimeCRem ?? consultation.DateTimeCRem;
    var diagnosis = request.Diagnosis ?? consultation.Diagnosis;

    // 3. Validaciones
    if (request.DateTimeCRem.HasValue && DateTime.Now < request.DateTimeCRem.Value)
        return Result<ConsultationReferralResponse>.Failure("La fecha de consulta no es válida");

    var head = await _departmentHeadRepository.GetByIdAsync(departmentHeadId);
    if (head == null)
        return Result<ConsultationReferralResponse>.Failure("El Jefe de Departamento especificado no existe");

    var doctor = await _doctorRepository.GetByIdAsync(doctorId);
    if (doctor == null || doctor.DepartmentId != head.DepartmentId)
        return Result<ConsultationReferralResponse>.Failure("El doctor tratante especificado no existe en el departamento consultante");

    var patient = await _referralRepository.GetByIdAsync(referralId);
    if (patient == null)
        return Result<ConsultationReferralResponse>.Failure("El paciente especificado no existe en remisión");

    // 4. ACTUALIZAR LA MISMA ENTIDAD TRACKADA
    consultation.Diagnosis = diagnosis;
    consultation.DepartmentHeadId = departmentHeadId;
    consultation.DoctorId = doctorId;
    consultation.ReferralId = referralId;
    consultation.DateTimeCRem = dateTimeCRem;

    // 5. Guardar cambios
    await _consultationReferralRepository.UpdateAsync(consultation);

    // 6. Recargar para enviar respuesta completa
    var updated = await _consultationReferralRepository.GetByIdAsync(id);

    var response = _mapper.Map<ConsultationReferralResponse>(updated);
    return Result<ConsultationReferralResponse>.Success(response);
}

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetByDateRangeAsync(
        Guid patientId, DateTime startDate, DateTime endDate)
    {
        var consultations = await _consultationReferralRepository.GetByDateRangeAsync(patientId, startDate, endDate);
        var response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(consultations);
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(response);
    }

    public async Task<Result<IEnumerable<ConsultationReferralResponse>>> GetLast10ByPatientIdAsync(Guid patientId)
    {
        var consultations = await _consultationReferralRepository.GetLast10ByPatientIdAsync(patientId);
        var response = _mapper.Map<IEnumerable<ConsultationReferralResponse>>(consultations);
        return Result<IEnumerable<ConsultationReferralResponse>>.Success(response);
    }
}