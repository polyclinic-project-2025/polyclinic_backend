using PolyclinicApplication.Common.Results;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicCore.Constants;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations;

public class EntityLinkingService : IEntityLinkingService
{
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Nurse> _nurseRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<WarehouseManager> _warehouseManagerRepository;

    public EntityLinkingService(
        IRepository<Doctor> doctorRepository,
        IRepository<Nurse> nurseRepository,
        IRepository<Patient> patientRepository,
        IRepository<WarehouseManager> warehouseManagerRepository)
    {
        _doctorRepository = doctorRepository;
        _nurseRepository = nurseRepository;
        _patientRepository = patientRepository;
        _warehouseManagerRepository = warehouseManagerRepository;
    }

    public async Task<Result<bool>> LinkEntityToUserAsync(Guid entityId, string userId, string role)
    {
        try
        {
            switch (role)
            {
                case ApplicationRoles.Doctor:
                    var doctor = await _doctorRepository.GetByIdAsync(entityId);
                    if (doctor == null)
                    return Result<bool>.Failure("Doctor no encontrado.");

                    doctor.UserId = userId;
                    await _doctorRepository.UpdateAsync(doctor);
                    break;

                case ApplicationRoles.Nurse:
                    var nurse = await _nurseRepository.GetByIdAsync(entityId);
                    if (nurse == null)
                        return Result<bool>.Failure("Enfermero no encontrado.");

                    nurse.UserId = userId;
                    await _nurseRepository.UpdateAsync(nurse);
                    break;

                case ApplicationRoles.Patient:
                    var patient = await _patientRepository.GetByIdAsync(entityId);
                    if (patient == null)
                        return Result<bool>.Failure("Paciente no encontrado.");

                    patient.UserId = userId;
                    await _patientRepository.UpdateAsync(patient);
                    break;
                case ApplicationRoles.WarehouseManager:
                    var warehouseManager = await _warehouseManagerRepository.GetByIdAsync(entityId);
                    if (warehouseManager == null)
                        return Result<bool>.Failure("Encargado de almacén no encontrado.");

                    warehouseManager.UserId = userId;
                    await _warehouseManagerRepository.UpdateAsync(warehouseManager);
                    break;

                default:
                    return Result<bool>.Failure($"Rol no soportado para vinculación: {role}");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al vincular entidad: {ex.Message}");
        }
    }
}