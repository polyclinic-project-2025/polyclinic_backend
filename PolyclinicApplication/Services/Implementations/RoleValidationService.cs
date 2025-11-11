using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicCore.Constants;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class RoleValidationService : IRoleValidationService
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Nurse> _nurseRepository;
    private readonly IRepository<MedicalStaff> _medicalStaffRepository;
    private readonly IRepository<WarehouseManager> _warehouseManagerRepository;
    private readonly IRepository<DepartmentHead> _departmentHeadRepository;
    private readonly IRepository<Patient> _patientRepository;
    public RoleValidationService(IIdentityService identityService,
        IRepository<Doctor> doctorRepository,
        IRepository<Nurse> nurseRepository,
        IRepository<MedicalStaff> medicalStaffRepository,
        IRepository<WarehouseManager> warehouseManagerRepository,
        IRepository<DepartmentHead> departmentHeadRepository,
        IRepository<Patient> patientRepository)
    {
        _identityService = identityService;
        _doctorRepository = doctorRepository;
        _nurseRepository = nurseRepository;
        _medicalStaffRepository = medicalStaffRepository;
        _warehouseManagerRepository = warehouseManagerRepository;
        _departmentHeadRepository = departmentHeadRepository;
        _patientRepository = patientRepository;
    }

    public async Task<Result<bool>> ValidateRolesExistAsync(IList<string> roles)
    {
        Console.WriteLine("Validating roles existence...");
        Console.WriteLine($"Roles to validate: {string.Join(", ", roles)}");
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

        // Regla 2: MedicalStaff es un rol genérico, no debe combinarse con Doctor o Nurse
        if (roles.Contains(ApplicationRoles.MedicalStaff))
        {
            if (roles.Contains(ApplicationRoles.Doctor) || roles.Contains(ApplicationRoles.Nurse))
            {
                return Result<bool>.Failure(
                    "El rol MedicalStaff no debe combinarse con Doctor o Nurse. Use roles específicos.");
            }
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ValidateRequiredDataForRoles(IList<string> roles, Dictionary<string, string>? validationData)
    {
        Console.WriteLine("Validating required data for roles...");
        Console.WriteLine($"Roles to validate: {string.Join(", ", roles)}");
        Console.WriteLine($"Validation data provided: {validationData != null && validationData.Any()}");
        if (roles == null || !roles.Any())
        {
            return Result<bool>.Failure("Debe proporcionar al menos un rol.");
        }

        foreach (var role in ApplicationRoles.AllRoles)
        {
            if (role != ApplicationRoles.Client && roles.Contains(role))
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
                            d => d.Identification.ToString() == data);

                        if (!doctors.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Doctor con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.Nurse:

                        var nurses = await _nurseRepository.FindAsync(
                            n => n.Identification.ToString() == data);

                        if (!nurses.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Enfermero con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.Patient:

                        var patients = await _patientRepository.FindAsync(
                            p => p.Identification.ToString() == data);

                        if (!patients.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Paciente con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.MedicalStaff:

                        var staff = await _medicalStaffRepository.FindAsync(
                            s => s.Identification.ToString() == data);

                        if (!staff.Any())
                        {
                            validationErrors.Add(
                                $"No existe Personal Médico con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.WarehouseManager:

                        var managers = await _warehouseManagerRepository.FindAsync(
                            w => w.Identification.ToString() == data);

                        if (!managers.Any())
                        {
                            validationErrors.Add(
                                $"No existe un Encargado de Almacén con el número de identificación: {data}");
                        }
                        break;

                    case ApplicationRoles.DepartmentHead:

                        var heads = await _departmentHeadRepository.FindAsync(
                            d => d.Identification.ToString() == data);

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
}