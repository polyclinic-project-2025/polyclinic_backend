using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicCore.Constants;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class RoleValidationService : IRoleValidationService
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Nurse> _nurseRepository;
    private readonly IRepository<WarehouseManager> _warehouseManagerRepository;
    private readonly IRepository<DepartmentHead> _departmentHeadRepository;
    private readonly IRepository<Patient> _patientRepository;
    public RoleValidationService(
        IRepository<Doctor> doctorRepository,
        IRepository<Nurse> nurseRepository,
        IRepository<WarehouseManager> warehouseManagerRepository,
        IRepository<DepartmentHead> departmentHeadRepository,
        IRepository<Patient> patientRepository)
    {
        _doctorRepository = doctorRepository;
        _nurseRepository = nurseRepository;
        _warehouseManagerRepository = warehouseManagerRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _patientRepository = patientRepository;
    }

    public async Task<Result<bool>> ValidateRolesExistAsync(IList<string> roles)
    {
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        var invalidRoles = new List<string>();

        foreach (var role in roles)
        {
            if (!ApplicationRoles.IsValidRole(role))
            {
                invalidRoles.Add(role);
            }
        }

        if (invalidRoles.Any())
        {
            return Result<bool>.Failure(
                $"Los siguientes roles no son válidos: {string.Join(", ", invalidRoles)}");
        }

        if (roles.Contains(ApplicationRoles.Admin)) return Result<bool>.Failure("No se permite el registro de administrador");

        return Result<bool>.Success(true);
    }

    public Result<bool> ValidateRolesCombination(IList<string> roles, Dictionary<string, string>? validationData = null)
    {
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        // Regla 1: Un usuario no puede ser Doctor y Nurse simultáneamente
        if (roles.Contains(ApplicationRoles.Doctor) && roles.Contains(ApplicationRoles.Nurse))
        {
            return Result<bool>.Failure(
                "Un usuario no puede tener los roles de Doctor y Nurse simultáneamente.");
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ValidateRequiredDataForRoles(IList<string> roles, Dictionary<string, string>? validationData)
    {
       
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        foreach (var role in ApplicationRoles.AllRoles)
        {
            if (roles.Contains(role))
            {
                if (validationData == null || !validationData.Any())
                {
                    return Result<bool>.Failure(
                        $"Se requieren datos adicionales para el rol {role}.");
                }
            }
        }

        if (validationData != null && validationData.Any())
        {
            var data = validationData["IdentificationNumber"];
            var validationErrors = new List<string>();
            foreach (var role in roles)
            {

                switch (role)
                {
                    case ApplicationRoles.Doctor:

                        var doctors = await _doctorRepository.FindAsync(
                            d => d.Identification == data);

                        if (!doctors.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Doctor con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.Nurse:

                        var nurses = await _nurseRepository.FindAsync(
                            n => n.Identification == data);

                        if (!nurses.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Enfermero con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.Patient:

                        var patients = await _patientRepository.FindAsync(
                            p => p.Identification == data);

                        if (!patients.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Paciente con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.WarehouseManager:

                        var managers = await _warehouseManagerRepository.FindAsync(
                            w => w.Identification == data);

                        if (!managers.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Encargado de Almacén con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.DepartmentHead:

                        var heads = await _departmentHeadRepository.FindAsync(
                            d => d.Identification == data);

                        if (!heads.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Jefe de Departamento con el número de identificación: {data}");
                        }
                        break;
                }
            }

            if (validationErrors.Any())
            {
                return Result<bool>.Failure(string.Join(" ", validationErrors));
            }
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<Guid>> ValidateEntityNotLinkedAsync(IList<string> roles, Dictionary<string, string>? validationData)
    {
        if (roles == null || !roles.Any())
        {
            return Result<Guid>.Failure("Debe proporcionar al menos un rol.");
        }

        if (validationData == null || !validationData.Any())
        {
            return Result<Guid>.Success(Guid.Empty);
        }

        Guid entityId = Guid.Empty;

        foreach (var role in roles)
        {
            switch (role)
            {
                case ApplicationRoles.Doctor:
                    if (validationData.TryGetValue("IdentificationNumber", out var doctorId))
                    {
                        var doctors = await _doctorRepository.FindAsync(
                            d => d.Identification == doctorId);
                        
                        var doctor = doctors.FirstOrDefault();
                        if (doctor == null)
                        {
                            return Result<Guid>.Failure(
                                $"No existe un Doctor con el número de identificación: {doctorId}");
                        }
                        
                        if (!string.IsNullOrEmpty(doctor.UserId))
                        {
                            return Result<Guid>.Failure(
                                "Este doctor ya tiene una cuenta de usuario asociada.");
                        }
                        
                        entityId = doctor.EmployeeId;
                    }
                    break;

                case ApplicationRoles.Nurse:
                    if (validationData.TryGetValue("IdentificationNumber", out var nurseId))
                    {
                        var nurses = await _nurseRepository.FindAsync(
                            n => n.Identification == nurseId);
                        
                        var nurse = nurses.FirstOrDefault();
                        if (nurse == null)
                        {
                            return Result<Guid>.Failure(
                                $"No existe un Enfermero con el número de identificación: {nurseId}");
                        }
                        
                        if (!string.IsNullOrEmpty(nurse.UserId))
                        {
                            return Result<Guid>.Failure(
                                "Este enfermero ya tiene una cuenta de usuario asociada.");
                        }
                        
                        entityId = nurse.EmployeeId;
                    }
                    break;

                case ApplicationRoles.Patient:
                    if (validationData.TryGetValue("IdentificationNumber", out var patientId))
                    {
                        var patients = await _patientRepository.FindAsync(
                            p => p.Identification == patientId);

                        var patient = patients.FirstOrDefault();
                        if (patient == null)
                        {
                            return Result<Guid>.Failure(
                                $"No existe un Paciente con el número de identificación: {patientId}");
                        }
                        if (!string.IsNullOrEmpty(patient.UserId))
                        {
                            return Result<Guid>.Failure(
                                "Este paciente ya tiene una cuenta de usuario asociada.");
                        }

                        entityId = patient.PatientId;
                        
                    }
                    break;
            }
        }

        return Result<Guid>.Success(entityId);
    }
}