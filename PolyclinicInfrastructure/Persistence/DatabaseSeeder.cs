using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PolyclinicCore.Constants;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DatabaseSeeder(
        AppDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Asegurar que la base de datos existe y está actualizada
            await _context.Database.MigrateAsync();

            // Verificar si ya hay datos sembrados
            if (await _context.Departments.AnyAsync())
            {
                Console.WriteLine("ℹ La base de datos ya contiene datos. Omitiendo seeding.");
                return;
            }

            // Sembrar datos en orden de dependencias
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDepartmentsAsync();
            await SeedExternalMedicalPostsAsync();
            await SeedPatientsAsync();
            await SeedDoctorsAsync();
            await LinkDoctorUserAsync(); // Vincular usuario doctor después de crear doctores
            await SeedNursesAsync();
            await LinkNurseUserAsync(); // Vincular usuario enfermero después de crear enfermeros
            await SeedWarehouseManagersAsync();
            await SeedDepartmentHeadsAsync();
            await SeedMedicationsAsync();
            await SeedStockDepartmentsAsync();
            await SeedDerivationsAsync();
            await SeedReferralsAsync();
            await SeedConsultationDerivationsAsync();
            await SeedConsultationReferralsAsync();
            await SeedMedicationDerivationsAsync();
            await SeedMedicationReferralsAsync();
            await SeedEmergencyRoomsAsync();
            await SeedEmergencyRoomCaresAsync();
            await SeedMedicationEmergenciesAsync();
            await SeedWarehouseRequestsAsync();
            await SeedMedicationRequestsAsync();

            Console.WriteLine("✓ Database seeding completado exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error durante el seeding: {ex.Message}");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in ApplicationRoles.AllRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"✓ Rol creado: {role}");
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        // Usuario Administrador
        var adminEmail = "admin@polyclinic.com";
        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
                Console.WriteLine($"✓ Usuario administrador creado: {adminEmail}");
            }
        }
    }

    private async Task LinkDoctorUserAsync()
    {
        // Vincular usuario doctor con la entidad Doctor
        var doctorEmail = "doctor@polyclinic.com";
        var doctorUser = await _userManager.FindByEmailAsync(doctorEmail);
        
        if (doctorUser == null)
        {
            doctorUser = new IdentityUser
            {
                UserName = doctorEmail,
                Email = doctorEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(doctorUser, "Doctor123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(doctorUser, ApplicationRoles.Doctor);
                Console.WriteLine($"✓ Usuario doctor creado: {doctorEmail}");
            }
        }

        // Buscar el primer doctor y vincularlo
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == null);
        if (doctor != null && doctorUser != null)
        {
            doctor.UserId = doctorUser.Id;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Usuario doctor vinculado con: {doctor.Name}");
        }
    }

    private async Task LinkNurseUserAsync()
    {
        // Vincular usuario nurse con la entidad Nurse
        var nurseEmail = "nurse@polyclinic.com";
        var nurseUser = await _userManager.FindByEmailAsync(nurseEmail);
        
        if (nurseUser == null)
        {
            nurseUser = new IdentityUser
            {
                UserName = nurseEmail,
                Email = nurseEmail,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(nurseUser, "Nurse123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(nurseUser, ApplicationRoles.Nurse);
                Console.WriteLine($"✓ Usuario enfermero creado: {nurseEmail}");
            }
        }

        // Buscar el primer enfermero y vincularlo
        var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == null);
        if (nurse != null && nurseUser != null)
        {
            nurse.UserId = nurseUser.Id;
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ Usuario enfermero vinculado con: {nurse.Name}");
        }
    }

    private async Task SeedDepartmentsAsync()
    {
        if (!await _context.Departments.AnyAsync())
        {
            var departments = new List<Department>
            {
                new Department(Guid.NewGuid(), "Cardiología"),
                new Department(Guid.NewGuid(), "Pediatría"),
                new Department(Guid.NewGuid(), "Medicina General"),
                new Department(Guid.NewGuid(), "Traumatología"),
                new Department(Guid.NewGuid(), "Ginecología"),
                new Department(Guid.NewGuid(), "Dermatología")
            };

            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {departments.Count} departamentos creados");
        }
    }

    private async Task SeedExternalMedicalPostsAsync()
    {
        if (!await _context.ExternalMedicalPosts.AnyAsync())
        {
            var posts = new List<ExternalMedicalPost>
            {
                new ExternalMedicalPost(Guid.NewGuid(), "Centro de Salud Norte"),
                new ExternalMedicalPost(Guid.NewGuid(), "Centro de Salud Sur"),
                new ExternalMedicalPost(Guid.NewGuid(), "Puesto Médico Este"),
                new ExternalMedicalPost(Guid.NewGuid(), "Clínica Rural Oeste"),
                new ExternalMedicalPost(Guid.NewGuid(), "Hospital Regional Central")
            };

            await _context.ExternalMedicalPosts.AddRangeAsync(posts);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {posts.Count} puestos médicos externos creados");
        }
    }

    private async Task SeedPatientsAsync()
    {
        if (!await _context.Patients.AnyAsync())
        {
            var patients = new List<Patient>
            {
                new Patient(Guid.NewGuid(), "Juan Pérez", "0123456789", 35, "0991234567", "Av. Principal 123, Quito"),
                new Patient(Guid.NewGuid(), "María García", "0234567890", 28, "0992345678", "Calle Secundaria 456, Guayaquil"),
                new Patient(Guid.NewGuid(), "Carlos Rodríguez", "0345678901", 42, "0993456789", "Barrio La Paz 789, Cuenca"),
                new Patient(Guid.NewGuid(), "Ana Martínez", "0456789012", 55, "0994567890", "Urbanización Los Álamos 321, Quito"),
                new Patient(Guid.NewGuid(), "Luis Fernández", "0567890123", 19, "0995678901", "Conjunto Habitacional 654, Ambato"),
                new Patient(Guid.NewGuid(), "Carmen López", "0678901234", 67, "0996789012", "Sector Norte 987, Riobamba"),
                new Patient(Guid.NewGuid(), "Pedro Sánchez", "0789012345", 31, "0997890123", "Ciudadela Sur 147, Machala"),
                new Patient(Guid.NewGuid(), "Laura Torres", "0890123456", 45, "0998901234", "Villa Florida 258, Loja")
            };

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {patients.Count} pacientes creados");
        }
    }

    private async Task SeedDoctorsAsync()
    {
        if (!await _context.Doctors.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            if (departments.Count == 0) return;

            var doctors = new List<Doctor>
            {
                new Doctor(Guid.NewGuid(), "1234567890", "Dr. Roberto Gómez", EmploymentStatus.Active, departments[0].DepartmentId),
                new Doctor(Guid.NewGuid(), "2345678901", "Dra. Elena Vásquez", EmploymentStatus.Active, departments[1].DepartmentId),
                new Doctor(Guid.NewGuid(), "3456789012", "Dr. Fernando Castro", EmploymentStatus.Active, departments[2].DepartmentId),
                new Doctor(Guid.NewGuid(), "4567890123", "Dra. Patricia Morales", EmploymentStatus.Active, departments[3].DepartmentId),
                new Doctor(Guid.NewGuid(), "5678901234", "Dr. Miguel Herrera", EmploymentStatus.Inactive, departments[4].DepartmentId),
                new Doctor(Guid.NewGuid(), "6789012345", "Dra. Sofía Ramírez", EmploymentStatus.Active, departments[5].DepartmentId)
            };

            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {doctors.Count} doctores creados");
        }
    }

    private async Task SeedNursesAsync()
    {
        if (!await _context.Nurses.AnyAsync())
        {
            var nurses = new List<Nurse>
            {
                new Nurse(Guid.NewGuid(), "7890123456", "Enf. Andrea Silva", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "8901234567", "Enf. Carlos Mendoza", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "9012345678", "Enf. Diana Ortiz", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "0123456780", "Enf. Eduardo Paredes", EmploymentStatus.Inactive),
                new Nurse(Guid.NewGuid(), "1234567891", "Enf. Gabriela Ruiz", EmploymentStatus.Active)
            };

            await _context.Nurses.AddRangeAsync(nurses);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {nurses.Count} enfermeros creados");
        }
    }

    private async Task SeedWarehouseManagersAsync()
    {
        if (!await _context.WarehouseManagers.AnyAsync())
        {
            var managers = new List<WarehouseManager>
            {
                new WarehouseManager(Guid.NewGuid(), "2345678902", "Ing. José Álvarez", EmploymentStatus.Active, DateTime.UtcNow.AddMonths(-6)),
                new WarehouseManager(Guid.NewGuid(), "3456789013", "Ing. Martha Jiménez", EmploymentStatus.Active, DateTime.UtcNow.AddMonths(-12))
            };

            await _context.WarehouseManagers.AddRangeAsync(managers);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {managers.Count} jefes de almacén creados");
        }
    }

    private async Task SeedDepartmentHeadsAsync()
    {
        if (!await _context.DepartmentHeads.AnyAsync())
        {
            var doctors = await _context.Doctors.Include(d => d.Department).ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            
            if (doctors.Count == 0 || departments.Count == 0) return;

            var heads = new List<DepartmentHead>();
            
            // Asignar los primeros doctores como jefes de sus respectivos departamentos
            for (int i = 0; i < Math.Min(doctors.Count, departments.Count); i++)
            {
                heads.Add(new DepartmentHead(
                    Guid.NewGuid(),
                    doctors[i].EmployeeId,
                    doctors[i].DepartmentId,
                    DateTime.UtcNow.AddMonths(-3)
                ));
            }

            await _context.DepartmentHeads.AddRangeAsync(heads);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {heads.Count} jefes de departamento asignados");
        }
    }

    private async Task SeedMedicationsAsync()
    {
        if (!await _context.Medications.AnyAsync())
        {
            var medications = new List<Medication>
            {
                new Medication(
                    Guid.NewGuid(),
                    "Tableta",
                    "Paracetamol Genérico",
                    "Laboratorios Ecuador",
                    "LOTE-2024-001",
                    "Acetaminofén",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(2)),
                    5000, 500, 1000, 100, 10000, 1000
                ),
                new Medication(
                    Guid.NewGuid(),
                    "Cápsula",
                    "Ibuprofeno 400mg",
                    "Farmacéutica Nacional",
                    "LOTE-2024-002",
                    "Ibuprofeno",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
                    3000, 300, 500, 50, 6000, 600
                ),
                new Medication(
                    Guid.NewGuid(),
                    "Jarabe",
                    "Amoxicilina 250mg/5ml",
                    "Laboratorios Vita",
                    "LOTE-2024-003",
                    "Amoxicilina",
                    DateOnly.FromDateTime(DateTime.Now.AddMonths(18)),
                    2000, 200, 300, 30, 4000, 400
                ),
                new Medication(
                    Guid.NewGuid(),
                    "Inyectable",
                    "Diclofenaco 75mg",
                    "Medicamentos SA",
                    "LOTE-2024-004",
                    "Diclofenaco sódico",
                    DateOnly.FromDateTime(DateTime.Now.AddMonths(24)),
                    1500, 150, 200, 20, 3000, 300
                ),
                new Medication(
                    Guid.NewGuid(),
                    "Tableta",
                    "Losartán 50mg",
                    "Cardio Pharma",
                    "LOTE-2024-005",
                    "Losartán potásico",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(3)),
                    4000, 400, 800, 80, 8000, 800
                ),
                new Medication(
                    Guid.NewGuid(),
                    "Crema",
                    "Betametasona 0.05%",
                    "Derma Labs",
                    "LOTE-2024-006",
                    "Betametasona",
                    DateOnly.FromDateTime(DateTime.Now.AddMonths(12)),
                    800, 80, 100, 10, 1500, 150
                )
            };

            await _context.Medications.AddRangeAsync(medications);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medications.Count} medicamentos creados");
        }
    }

    private async Task SeedStockDepartmentsAsync()
    {
        if (!await _context.StockDepartments.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            var medications = await _context.Medications.ToListAsync();

            if (departments.Count == 0 || medications.Count == 0) return;

            var stocks = new List<StockDepartment>();

            // Asignar stock de los primeros 3 medicamentos a cada departamento
            foreach (var dept in departments)
            {
                for (int i = 0; i < Math.Min(3, medications.Count); i++)
                {
                    stocks.Add(new StockDepartment(
                        Guid.NewGuid(),
                        200 + (i * 50), // Cantidad variada
                        dept.DepartmentId,
                        medications[i].MedicationId,
                        50,  // Min
                        500  // Max
                    ));
                }
            }

            await _context.StockDepartments.AddRangeAsync(stocks);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {stocks.Count} registros de stock departamental creados");
        }
    }

    private async Task SeedDerivationsAsync()
    {
        if (!await _context.Derivations.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            var patients = await _context.Patients.ToListAsync();

            if (departments.Count < 2 || patients.Count == 0) return;

            var derivations = new List<Derivation>
            {
                new Derivation(
                    Guid.NewGuid(),
                    departments[0].DepartmentId, // De Cardiología
                    DateTime.UtcNow.AddDays(-5),
                    patients[0].PatientId,
                    departments[1].DepartmentId  // A Pediatría
                ),
                new Derivation(
                    Guid.NewGuid(),
                    departments[2].DepartmentId, // De Medicina General
                    DateTime.UtcNow.AddDays(-3),
                    patients[1].PatientId,
                    departments[3].DepartmentId  // A Traumatología
                ),
                new Derivation(
                    Guid.NewGuid(),
                    departments[1].DepartmentId,
                    DateTime.UtcNow.AddDays(-1),
                    patients[2].PatientId,
                    departments[0].DepartmentId
                )
            };

            await _context.Derivations.AddRangeAsync(derivations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {derivations.Count} derivaciones creadas");
        }
    }

    private async Task SeedReferralsAsync()
    {
        if (!await _context.Referrals.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            var patients = await _context.Patients.ToListAsync();
            var externalPosts = await _context.ExternalMedicalPosts.ToListAsync();

            if (departments.Count == 0 || patients.Count == 0 || externalPosts.Count == 0) return;

            var referrals = new List<Referral>
            {
                new Referral(
                    Guid.NewGuid(),
                    patients[3].PatientId,
                    DateTime.UtcNow.AddDays(-7),
                    externalPosts[0].ExternalMedicalPostId,
                    departments[0].DepartmentId
                ),
                new Referral(
                    Guid.NewGuid(),
                    patients[4].PatientId,
                    DateTime.UtcNow.AddDays(-4),
                    externalPosts[1].ExternalMedicalPostId,
                    departments[2].DepartmentId
                ),
                new Referral(
                    Guid.NewGuid(),
                    patients[5].PatientId,
                    DateTime.UtcNow.AddDays(-2),
                    externalPosts[2].ExternalMedicalPostId,
                    departments[4].DepartmentId
                )
            };

            await _context.Referrals.AddRangeAsync(referrals);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {referrals.Count} remisiones creadas");
        }
    }

    private async Task SeedConsultationDerivationsAsync()
    {
        if (!await _context.ConsultationDerivations.AnyAsync())
        {
            var derivations = await _context.Derivations.ToListAsync();
            var doctors = await _context.Doctors.Where(d => d.EmploymentStatus == EmploymentStatus.Active).ToListAsync();
            var departmentHeads = await _context.DepartmentHeads.ToListAsync();

            if (derivations.Count == 0 || doctors.Count == 0 || departmentHeads.Count == 0) return;

            var consultations = new List<ConsultationDerivation>();

            for (int i = 0; i < Math.Min(derivations.Count, doctors.Count); i++)
            {
                consultations.Add(new ConsultationDerivation(
                    Guid.NewGuid(),
                    $"Diagnóstico de derivación {i + 1}: Requiere seguimiento especializado",
                    derivations[i].DerivationId,
                    DateTime.UtcNow.AddDays(-i),
                    doctors[i].EmployeeId,
                    departmentHeads[Math.Min(i, departmentHeads.Count - 1)].DepartmentHeadId
                ));
            }

            await _context.ConsultationDerivations.AddRangeAsync(consultations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {consultations.Count} consultas de derivación creadas");
        }
    }

    private async Task SeedConsultationReferralsAsync()
    {
        if (!await _context.ConsultationReferrals.AnyAsync())
        {
            var referrals = await _context.Referrals.ToListAsync();
            var doctors = await _context.Doctors.Where(d => d.EmploymentStatus == EmploymentStatus.Active).ToListAsync();
            var departmentHeads = await _context.DepartmentHeads.ToListAsync();

            if (referrals.Count == 0 || doctors.Count == 0 || departmentHeads.Count == 0) return;

            var consultations = new List<ConsultationReferral>();

            for (int i = 0; i < Math.Min(referrals.Count, doctors.Count); i++)
            {
                consultations.Add(new ConsultationReferral(
                    Guid.NewGuid(),
                    $"Diagnóstico de remisión {i + 1}: Atención especializada requerida desde puesto externo",
                    departmentHeads[Math.Min(i, departmentHeads.Count - 1)].DepartmentHeadId,
                    doctors[i].EmployeeId,
                    referrals[i].ReferralId,
                    DateTime.UtcNow.AddDays(-i - 1)
                ));
            }

            await _context.ConsultationReferrals.AddRangeAsync(consultations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {consultations.Count} consultas de remisión creadas");
        }
    }

    private async Task SeedMedicationDerivationsAsync()
    {
        if (!await _context.MedicationDerivations.AnyAsync())
        {
            var consultationDerivations = await _context.ConsultationDerivations.ToListAsync();
            var medications = await _context.Medications.ToListAsync();

            if (consultationDerivations.Count == 0 || medications.Count == 0) return;

            var medDerivations = new List<MedicationDerivation>();

            foreach (var consultation in consultationDerivations)
            {
                // Asignar 1-2 medicamentos por consulta
                for (int i = 0; i < Math.Min(2, medications.Count); i++)
                {
                    medDerivations.Add(new MedicationDerivation(
                        Guid.NewGuid(),
                        10 + (i * 5), // Cantidad variada
                        consultation.ConsultationDerivationId,
                        medications[i].MedicationId
                    ));
                }
            }

            await _context.MedicationDerivations.AddRangeAsync(medDerivations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medDerivations.Count} medicamentos de derivación creados");
        }
    }

    private async Task SeedMedicationReferralsAsync()
    {
        if (!await _context.MedicationReferrals.AnyAsync())
        {
            var consultationReferrals = await _context.ConsultationReferrals.ToListAsync();
            var medications = await _context.Medications.ToListAsync();

            if (consultationReferrals.Count == 0 || medications.Count == 0) return;

            var medReferrals = new List<MedicationReferral>();

            foreach (var consultation in consultationReferrals)
            {
                // Asignar 1-3 medicamentos por consulta
                for (int i = 0; i < Math.Min(3, medications.Count); i++)
                {
                    medReferrals.Add(new MedicationReferral(
                        Guid.NewGuid(),
                        15 + (i * 7), // Cantidad variada
                        consultation.ConsultationReferralId,
                        medications[i].MedicationId
                    ));
                }
            }

            await _context.MedicationReferrals.AddRangeAsync(medReferrals);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medReferrals.Count} medicamentos de remisión creados");
        }
    }

    private async Task SeedEmergencyRoomsAsync()
    {
        if (!await _context.EmergencyRooms.AnyAsync())
        {
            var doctors = await _context.Doctors.Where(d => d.EmploymentStatus == EmploymentStatus.Active).ToListAsync();

            if (doctors.Count == 0) return;

            var emergencyRooms = new List<EmergencyRoom>();

            // Crear guardias para los próximos días
            for (int i = 0; i < Math.Min(5, doctors.Count); i++)
            {
                emergencyRooms.Add(new EmergencyRoom(
                    Guid.NewGuid(),
                    doctors[i].EmployeeId,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(i))
                ));
            }

            await _context.EmergencyRooms.AddRangeAsync(emergencyRooms);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {emergencyRooms.Count} guardias de emergencia creadas");
        }
    }

    private async Task SeedEmergencyRoomCaresAsync()
    {
        if (!await _context.EmergencyRoomCares.AnyAsync())
        {
            var emergencyRooms = await _context.EmergencyRooms.ToListAsync();
            var patients = await _context.Patients.ToListAsync();

            if (emergencyRooms.Count == 0 || patients.Count == 0) return;

            var cares = new List<EmergencyRoomCare>();

            for (int i = 0; i < Math.Min(emergencyRooms.Count, patients.Count); i++)
            {
                cares.Add(new EmergencyRoomCare(
                    Guid.NewGuid(),
                    $"Atención de emergencia {i + 1}: Evaluación y estabilización del paciente",
                    emergencyRooms[i].EmergencyRoomId,
                    DateTime.UtcNow.AddHours(-i * 2),
                    patients[i].PatientId
                ));
            }

            await _context.EmergencyRoomCares.AddRangeAsync(cares);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {cares.Count} atenciones de emergencia creadas");
        }
    }

    private async Task SeedMedicationEmergenciesAsync()
    {
        if (!await _context.MedicationEmergency.AnyAsync())
        {
            var emergencyCares = await _context.EmergencyRoomCares.ToListAsync();
            var medications = await _context.Medications.ToListAsync();

            if (emergencyCares.Count == 0 || medications.Count == 0) return;

            var medEmergencies = new List<MedicationEmergency>();

            foreach (var care in emergencyCares)
            {
                // Asignar 1-2 medicamentos por atención de emergencia
                for (int i = 0; i < Math.Min(2, medications.Count); i++)
                {
                    medEmergencies.Add(new MedicationEmergency(
                        Guid.NewGuid(),
                        5 + (i * 3), // Cantidad variada
                        care.EmergencyRoomCareId,
                        medications[i].MedicationId
                    ));
                }
            }

            await _context.MedicationEmergency.AddRangeAsync(medEmergencies);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medEmergencies.Count} medicamentos de emergencia creados");
        }
    }

    private async Task SeedWarehouseRequestsAsync()
    {
        if (!await _context.WarehouseRequests.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            var managers = await _context.WarehouseManagers.ToListAsync();

            if (departments.Count == 0 || managers.Count == 0) return;

            var requests = new List<WarehouseRequest>();

            for (int i = 0; i < Math.Min(4, departments.Count); i++)
            {
                var status = i % 2 == 0 ? "Aprobado" : "Pendiente";
                requests.Add(new WarehouseRequest(
                    Guid.NewGuid(),
                    status,
                    DateTime.UtcNow.AddDays(-i - 1),
                    departments[i].DepartmentId,
                    managers[i % managers.Count].EmployeeId
                ));
            }

            await _context.WarehouseRequests.AddRangeAsync(requests);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {requests.Count} solicitudes de almacén creadas");
        }
    }

    private async Task SeedMedicationRequestsAsync()
    {
        if (!await _context.MedicationRequests.AnyAsync())
        {
            var warehouseRequests = await _context.WarehouseRequests.ToListAsync();
            var medications = await _context.Medications.ToListAsync();

            if (warehouseRequests.Count == 0 || medications.Count == 0) return;

            var medRequests = new List<MedicationRequest>();

            foreach (var request in warehouseRequests)
            {
                // Asignar 2-4 medicamentos por solicitud
                for (int i = 0; i < Math.Min(3, medications.Count); i++)
                {
                    medRequests.Add(new MedicationRequest(
                        Guid.NewGuid(),
                        50 + (i * 25), // Cantidad variada
                        request.WarehouseRequestId,
                        medications[i].MedicationId
                    ));
                }
            }

            await _context.MedicationRequests.AddRangeAsync(medRequests);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medRequests.Count} solicitudes de medicamentos creadas");
        }
    }
}
