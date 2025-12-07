using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

/// <summary>
/// Implementación optimizada del repositorio de perfiles de usuario.
/// Usa consultas eficientes para minimizar llamadas a la base de datos.
/// </summary>
public class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _context;

    public UserProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetDoctorByUserIdAsync(string userId)
    {
        return await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<Nurse?> GetNurseByUserIdAsync(string userId)
    {
        return await _context.Nurses
            .FirstOrDefaultAsync(n => n.UserId == userId);
    }

    public async Task<WarehouseManager?> GetWarehouseManagerByUserIdAsync(string userId)
    {
        return await _context.WarehouseManagers
            .Include(w => w.Warehouse)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<Patient?> GetPatientByUserIdAsync(string userId)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<DepartmentHead?> GetDepartmentHeadByDoctorIdAsync(Guid doctorId)
    {
        return await _context.DepartmentHeads
            .Include(dh => dh.Department)
            .FirstOrDefaultAsync(dh => dh.DoctorId == doctorId);
    }

    public async Task<string?> GetLinkedEntityTypeAsync(string userId)
    {
        // Consulta eficiente: verifica existencia en orden de prioridad
        // Doctor > Nurse > WarehouseManager > Patient
        
        if (await _context.Doctors.AnyAsync(d => d.UserId == userId))
            return "Doctor";
        
        if (await _context.Nurses.AnyAsync(n => n.UserId == userId))
            return "Nurse";
        
        if (await _context.WarehouseManagers.AnyAsync(w => w.UserId == userId))
            return "WarehouseManager";
        
        if (await _context.Patients.AnyAsync(p => p.UserId == userId))
            return "Patient";
        
        return null;
    }

    public async Task<UserProfileData?> GetUserProfileDataAsync(string userId)
    {
        // Estrategia optimizada: primero determinar el tipo con consultas ligeras,
        // luego traer solo los datos necesarios con los includes correctos.
        
        // 1. Intentar Doctor (incluye Department y verificar DepartmentHead)
        var doctor = await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.UserId == userId);
        
        if (doctor != null)
        {
            var departmentHead = await _context.DepartmentHeads
                .FirstOrDefaultAsync(dh => dh.DoctorId == doctor.EmployeeId);
            
            return new UserProfileData
            {
                UserId = userId,
                EntityType = "Doctor",
                EmployeeId = doctor.EmployeeId,
                Identification = doctor.Identification,
                Name = doctor.Name,
                EmploymentStatus = doctor.EmploymentStatus,
                DepartmentId = doctor.DepartmentId,
                DepartmentName = doctor.Department?.Name,
                IsDepartmentHead = departmentHead != null,
                DepartmentHeadAssignedAt = departmentHead?.AssignedAt
            };
        }

        // 2. Intentar Nurse
        var nurse = await _context.Nurses
            .FirstOrDefaultAsync(n => n.UserId == userId);
        
        if (nurse != null)
        {
            return new UserProfileData
            {
                UserId = userId,
                EntityType = "Nurse",
                EmployeeId = nurse.EmployeeId,
                Identification = nurse.Identification,
                Name = nurse.Name,
                EmploymentStatus = nurse.EmploymentStatus
            };
        }

        // 3. Intentar WarehouseManager (incluye Warehouse)
        var warehouseManager = await _context.WarehouseManagers
            .Include(w => w.Warehouse)
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (warehouseManager != null)
        {
            return new UserProfileData
            {
                UserId = userId,
                EntityType = "WarehouseManager",
                EmployeeId = warehouseManager.EmployeeId,
                Identification = warehouseManager.Identification,
                Name = warehouseManager.Name,
                EmploymentStatus = warehouseManager.EmploymentStatus,
                WarehouseId = warehouseManager.WarehouseId,
                WarehouseName = warehouseManager.Warehouse?.Name
            };
        }

        // 4. Intentar Patient
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (patient != null)
        {
            return new UserProfileData
            {
                UserId = userId,
                EntityType = "Patient",
                PatientId = patient.PatientId,
                Identification = patient.Identification,
                Name = patient.Name,
                Age = patient.Age,
                Contact = patient.Contact,
                Address = patient.Address
            };
        }

        return null;
    }
}
