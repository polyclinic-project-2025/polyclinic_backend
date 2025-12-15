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

            // Aplicar triggers de validación (siempre se ejecuta, usa CREATE OR REPLACE)
            await _context.ApplyDatabaseTriggersAsync();
            Console.WriteLine("✓ Triggers de validación aplicados");

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
            
            // ========== CONSULTAS DE SEGUIMIENTO PARA PACIENTES MAYORES ==========
            await SeedElderlyPatientFollowUpConsultationsAsync();

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
                // 6 departamentos originales
                new Department(Guid.NewGuid(), "Cardiología"),
                new Department(Guid.NewGuid(), "Pediatría"),
                new Department(Guid.NewGuid(), "Medicina General"),
                new Department(Guid.NewGuid(), "Traumatología"),
                new Department(Guid.NewGuid(), "Ginecología"),
                new Department(Guid.NewGuid(), "Dermatología"),
                // 10 departamentos adicionales para pacientes mayores
                new Department(Guid.NewGuid(), "Geriatría"),
                new Department(Guid.NewGuid(), "Neurología"),
                new Department(Guid.NewGuid(), "Neumología"),
                new Department(Guid.NewGuid(), "Nefrología"),
                new Department(Guid.NewGuid(), "Reumatología"),
                new Department(Guid.NewGuid(), "Endocrinología"),
                new Department(Guid.NewGuid(), "Gastroenterología"),
                new Department(Guid.NewGuid(), "Urología"),
                new Department(Guid.NewGuid(), "Oftalmología"),
                new Department(Guid.NewGuid(), "Oncología")
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
            // Crear 200 pacientes para cubrir las consultas de 5 meses (15 doctores * ~5-10 consultas/mes)
            var firstNames = new[] { "Juan", "María", "Carlos", "Ana", "Luis", "Carmen", "Pedro", "Laura", 
                "Miguel", "Sofía", "Andrés", "Elena", "Fernando", "Patricia", "Roberto", "Diana",
                "José", "Gabriela", "Ricardo", "Valeria", "Daniel", "Camila", "Santiago", "Isabella",
                "Diego", "Natalia", "Alejandro", "Lucía", "Sebastián", "Paula", "Mateo", "Daniela",
                "Nicolás", "Mariana", "David", "Carolina", "Eduardo", "Andrea", "Gabriel", "Fernanda" };
            
            var lastNames = new[] { "Pérez", "García", "Rodríguez", "Martínez", "López", "González", 
                "Sánchez", "Torres", "Ramírez", "Flores", "Vargas", "Castro", "Morales", "Herrera",
                "Ortiz", "Silva", "Núñez", "Mendoza", "Ruiz", "Jiménez", "Álvarez", "Romero",
                "Díaz", "Medina", "Guzmán", "Campos", "Rojas", "Cruz", "Reyes", "Vega" };
            
            var cities = new[] { "Quito", "Guayaquil", "Cuenca", "Ambato", "Riobamba", "Machala", "Loja", "Manta", "Portoviejo", "Esmeraldas" };
            var streets = new[] { "Av. Principal", "Calle Secundaria", "Barrio La Paz", "Urbanización Los Álamos", 
                "Conjunto Habitacional", "Sector Norte", "Ciudadela Sur", "Villa Florida", "Centro Histórico", "Zona Comercial" };

            var patients = new List<Patient>();
            var random = new Random(42); // Seed fijo para reproducibilidad

            for (int i = 0; i < 200; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName1 = lastNames[random.Next(lastNames.Length)];
                var lastName2 = lastNames[random.Next(lastNames.Length)];
                var fullName = $"{firstName} {lastName1} {lastName2}";
                
                // Generar cédula única de 10 dígitos
                var identification = $"{random.Next(10):D1}{random.Next(100000000, 999999999)}";
                
                var age = random.Next(1, 90);
                var contact = $"09{random.Next(90000000, 99999999)}";
                var address = $"{streets[random.Next(streets.Length)]} {random.Next(100, 999)}, {cities[random.Next(cities.Length)]}";

                patients.Add(new Patient(Guid.NewGuid(), fullName, identification, age, contact, address));
            }

            // ========== 50 PACIENTES MAYORES (60-90 años) ==========
            var elderlyFirstNames = new[] { "Alberto", "Ramiro", "Germán", "Oswaldo", "Humberto", 
                "Gloria", "Esperanza", "Dolores", "Blanca", "Mercedes", "Augusto", "Reinaldo",
                "Ernestina", "Zoila", "Clotilde", "Segundo", "Heriberto", "Celso", "Rosario", "Amelia" };
            
            var elderlyRandom = new Random(999); // Seed diferente para pacientes mayores
            
            for (int i = 0; i < 50; i++)
            {
                var firstName = elderlyFirstNames[elderlyRandom.Next(elderlyFirstNames.Length)];
                var lastName1 = lastNames[elderlyRandom.Next(lastNames.Length)];
                var lastName2 = lastNames[elderlyRandom.Next(lastNames.Length)];
                var fullName = $"{firstName} {lastName1} {lastName2}";
                
                // Cédulas con prefijo especial para identificarlos fácilmente
                var identification = $"50{i:D2}{elderlyRandom.Next(100000, 999999)}";
                
                // Edad entre 60 y 90 años
                var age = elderlyRandom.Next(60, 91);
                var contact = $"02{elderlyRandom.Next(2000000, 2999999)}"; // Teléfonos fijos
                var address = $"{streets[elderlyRandom.Next(streets.Length)]} {elderlyRandom.Next(100, 999)}, {cities[elderlyRandom.Next(cities.Length)]}";

                patients.Add(new Patient(Guid.NewGuid(), fullName, identification, age, contact, address));
            }

            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {patients.Count} pacientes creados (incluye 50 pacientes mayores 60-90 años)");
        }
    }

    private async Task SeedDoctorsAsync()
    {
        if (!await _context.Doctors.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            if (departments.Count == 0) return;

            // Lista de doctores con datos coherentes distribuidos en los 16 departamentos
            var doctorData = new List<(string Identification, string Name, int DeptIndex)>
            {
                // Cardiología (índice 0) - 3 doctores
                ("1234567890", "Dr. Roberto Gómez Mendoza", 0),
                ("1234567891", "Dra. Carmen Lucia Ortega", 0),
                ("1234567892", "Dr. Andrés Felipe Mora", 0),
                
                // Pediatría (índice 1) - 3 doctores
                ("2345678901", "Dra. Elena Vásquez Ruiz", 1),
                ("2345678902", "Dr. José Manuel Paredes", 1),
                ("2345678903", "Dra. Gabriela Mendez Torres", 1),
                
                // Medicina General (índice 2) - 3 doctores
                ("3456789012", "Dr. Fernando Castro Ríos", 2),
                ("3456789013", "Dra. Lucía Salazar Vega", 2),
                ("3456789014", "Dr. Ricardo Núñez Peña", 2),
                
                // Traumatología (índice 3) - 2 doctores
                ("4567890123", "Dra. Patricia Morales Luna", 3),
                ("4567890124", "Dr. Sebastián Rojas Cárdenas", 3),
                
                // Ginecología (índice 4) - 2 doctores
                ("5678901234", "Dr. Miguel Herrera Solano", 4),
                ("5678901235", "Dra. Valeria Campos Mejía", 4),
                
                // Dermatología (índice 5) - 2 doctores
                ("6789012345", "Dra. Sofía Ramírez Guerrero", 5),
                ("6789012346", "Dr. Daniel Espinoza Cruz", 5),
                
                // ========== NUEVOS DEPARTAMENTOS (para pacientes mayores) ==========
                
                // Geriatría (índice 6) - 2 doctores
                ("9001000001", "Dr. Hernán Villacís Mora", 6),
                ("9001000002", "Dra. Beatriz Córdova Luna", 6),
                
                // Neurología (índice 7) - 2 doctores
                ("9001000003", "Dr. Mauricio Estrella Paz", 7),
                ("9001000004", "Dra. Cristina Burbano Vega", 7),
                
                // Neumología (índice 8) - 2 doctores
                ("9001000005", "Dr. Gonzalo Terán Mejía", 8),
                ("9001000006", "Dra. Mónica Carvajal Ruiz", 8),
                
                // Nefrología (índice 9) - 2 doctores
                ("9001000007", "Dr. Óscar Maldonado Cruz", 9),
                ("9001000008", "Dra. Silvia Proaño Díaz", 9),
                
                // Reumatología (índice 10) - 2 doctores
                ("9001000009", "Dr. Arturo Benítez Lara", 10),
                ("9001000010", "Dra. Lorena Jaramillo Paz", 10),
                
                // Endocrinología (índice 11) - 2 doctores
                ("9001000011", "Dr. Raúl Espinoza Vaca", 11),
                ("9001000012", "Dra. Diana Cevallos Mora", 11),
                
                // Gastroenterología (índice 12) - 2 doctores
                ("9001000013", "Dr. Fabián Narváez Ríos", 12),
                ("9001000014", "Dra. Adriana Suárez Luna", 12),
                
                // Urología (índice 13) - 2 doctores
                ("9001000015", "Dr. Ernesto Calderón Vega", 13),
                ("9001000016", "Dra. Paola Rivas Mejía", 13),
                
                // Oftalmología (índice 14) - 2 doctores
                ("9001000017", "Dr. Víctor Aguirre Paz", 14),
                ("9001000018", "Dra. Margarita León Cruz", 14),
                
                // Oncología (índice 15) - 2 doctores
                ("9001000019", "Dr. César Paredes Díaz", 15),
                ("9001000020", "Dra. Verónica Salazar Mora", 15)
            };

            var doctors = doctorData.Select(d => new Doctor(
                Guid.NewGuid(),
                d.Identification,
                d.Name,
                EmploymentStatus.Active,
                departments[d.DeptIndex].DepartmentId
            )).ToList();

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
                new Nurse(Guid.NewGuid(), "7001234561", "Enf. Andrea Silva Moreno", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "7001234562", "Enf. Carlos Mendoza Ríos", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "7001234563", "Enf. Diana Ortiz Luna", EmploymentStatus.Active),
                new Nurse(Guid.NewGuid(), "7001234564", "Enf. Eduardo Paredes Vega", EmploymentStatus.Inactive),
                new Nurse(Guid.NewGuid(), "7001234565", "Enf. Gabriela Ruiz Castro", EmploymentStatus.Active)
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
                new WarehouseManager(Guid.NewGuid(), "8001234561", "Ing. José Álvarez Medina", EmploymentStatus.Active, DateTime.UtcNow.AddMonths(-6)),
                new WarehouseManager(Guid.NewGuid(), "8001234562", "Ing. Martha Jiménez Soto", EmploymentStatus.Active, DateTime.UtcNow.AddMonths(-12))
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
            var departments = await _context.Departments.ToListAsync();
            var doctors = await _context.Doctors
                .Where(d => d.EmploymentStatus == EmploymentStatus.Active)
                .ToListAsync();
            
            if (doctors.Count == 0 || departments.Count == 0) return;

            var heads = new List<DepartmentHead>();
            
            // Asignar un jefe de departamento por cada departamento
            // Seleccionamos el primer doctor de cada departamento como jefe
            foreach (var department in departments)
            {
                var doctorInDept = doctors.FirstOrDefault(d => d.DepartmentId == department.DepartmentId);
                
                if (doctorInDept != null)
                {
                    heads.Add(new DepartmentHead(
                        Guid.NewGuid(),
                        doctorInDept.EmployeeId,
                        department.DepartmentId,
                        DateTime.UtcNow.AddMonths(-6) // Asignado hace 6 meses
                    ));
                    Console.WriteLine($"✓ Jefe de departamento asignado: {doctorInDept.Name} -> {department.Name}");
                }
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

            // Asignar stock de TODOS los medicamentos a CADA departamento
            // Esto simula un inventario inicial completo antes de las consultas
            foreach (var dept in departments)
            {
                foreach (var med in medications)
                {
                    stocks.Add(new StockDepartment(
                        Guid.NewGuid(),
                        500, // Cantidad inicial alta para soportar las recetas
                        dept.DepartmentId,
                        med.MedicationId,
                        50,  // Min
                        1000 // Max
                    ));
                }
            }

            await _context.StockDepartments.AddRangeAsync(stocks);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {stocks.Count} registros de stock departamental creados (todos los medicamentos en todos los departamentos)");
        }
    }

    private async Task SeedDerivationsAsync()
    {
        if (!await _context.Derivations.AnyAsync())
        {
            var departments = await _context.Departments.ToListAsync();
            var patients = await _context.Patients.ToListAsync();

            if (departments.Count < 2 || patients.Count == 0) return;

            var derivations = new List<Derivation>();
            var random = new Random(123); // Seed fijo para reproducibilidad
            var patientIndex = 0;

            // Crear derivaciones para los últimos 5 meses
            // Cada departamento genera derivaciones hacia otros departamentos
            for (int monthOffset = 0; monthOffset < 5; monthOffset++)
            {
                var baseDate = DateTime.UtcNow.AddMonths(-monthOffset);
                var daysInMonth = DateTime.DaysInMonth(baseDate.Year, baseDate.Month);

                // Generar entre 20-30 derivaciones por mes
                var derivationsThisMonth = random.Next(20, 31);
                
                for (int i = 0; i < derivationsThisMonth; i++)
                {
                    var fromDeptIndex = random.Next(departments.Count);
                    var toDeptIndex = random.Next(departments.Count);
                    
                    // Asegurar que origen y destino sean diferentes
                    while (toDeptIndex == fromDeptIndex)
                    {
                        toDeptIndex = random.Next(departments.Count);
                    }

                    var dayOfMonth = random.Next(1, Math.Min(daysInMonth, 28) + 1);
                    var hour = random.Next(8, 18); // Entre 8am y 6pm
                    var derivationDate = new DateTime(baseDate.Year, baseDate.Month, dayOfMonth, hour, random.Next(0, 60), 0, DateTimeKind.Utc);

                    derivations.Add(new Derivation(
                        Guid.NewGuid(),
                        departments[fromDeptIndex].DepartmentId,
                        derivationDate,
                        patients[patientIndex % patients.Count].PatientId,
                        departments[toDeptIndex].DepartmentId
                    ));

                    patientIndex++;
                }
            }

            await _context.Derivations.AddRangeAsync(derivations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {derivations.Count} derivaciones creadas para 5 meses");
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

            var referrals = new List<Referral>();
            var random = new Random(456); // Seed fijo diferente para variedad
            var patientIndex = 100; // Comenzar desde otro índice para usar diferentes pacientes

            // Crear remisiones para los últimos 5 meses
            for (int monthOffset = 0; monthOffset < 5; monthOffset++)
            {
                var baseDate = DateTime.UtcNow.AddMonths(-monthOffset);
                var daysInMonth = DateTime.DaysInMonth(baseDate.Year, baseDate.Month);

                // Generar entre 25-35 remisiones por mes (desde puestos externos)
                var referralsThisMonth = random.Next(25, 36);
                
                for (int i = 0; i < referralsThisMonth; i++)
                {
                    var externalPostIndex = random.Next(externalPosts.Count);
                    var toDeptIndex = random.Next(departments.Count);

                    var dayOfMonth = random.Next(1, Math.Min(daysInMonth, 28) + 1);
                    var hour = random.Next(7, 20); // Entre 7am y 8pm
                    var referralDate = new DateTime(baseDate.Year, baseDate.Month, dayOfMonth, hour, random.Next(0, 60), 0, DateTimeKind.Utc);

                    referrals.Add(new Referral(
                        Guid.NewGuid(),
                        patients[patientIndex % patients.Count].PatientId,
                        referralDate,
                        externalPosts[externalPostIndex].ExternalMedicalPostId,
                        departments[toDeptIndex].DepartmentId
                    ));

                    patientIndex++;
                }
            }

            await _context.Referrals.AddRangeAsync(referrals);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {referrals.Count} remisiones creadas para 5 meses");
        }
    }

    private async Task SeedConsultationDerivationsAsync()
    {
        if (!await _context.ConsultationDerivations.AnyAsync())
        {
            var derivations = await _context.Derivations
                .Include(d => d.DepartmentTo)
                .ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.EmploymentStatus == EmploymentStatus.Active)
                .ToListAsync();
            var departmentHeads = await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .ToListAsync();

            if (derivations.Count == 0 || doctors.Count == 0 || departmentHeads.Count == 0) return;

            var consultations = new List<ConsultationDerivation>();
            var diagnoses = new[]
            {
                "Paciente presenta síntomas estables, se recomienda seguimiento periódico",
                "Evaluación completa realizada, requiere tratamiento especializado",
                "Diagnóstico confirmado, se inicia protocolo de tratamiento",
                "Control de rutina satisfactorio, continuar medicación actual",
                "Se detectan signos de mejora, reducir dosis gradualmente",
                "Requiere estudios complementarios para diagnóstico definitivo",
                "Condición estabilizada, alta con indicaciones de cuidado",
                "Evolución favorable, próxima cita en 30 días",
                "Se ajusta tratamiento según resultados de laboratorio",
                "Paciente respondiendo bien al tratamiento, mantener seguimiento"
            };
            
            var random = new Random(789);

            // Agrupar derivaciones por departamento destino
            var derivationsByDept = derivations.GroupBy(d => d.DepartmentToId).ToList();

            foreach (var deptGroup in derivationsByDept)
            {
                var deptId = deptGroup.Key;
                
                // Buscar el DepartmentHead del departamento destino
                var matchingHead = departmentHeads.FirstOrDefault(dh => dh.DepartmentId == deptId);
                if (matchingHead == null) continue;

                // Obtener doctores del departamento destino
                var deptDoctors = doctors.Where(d => d.DepartmentId == deptId).ToList();
                if (deptDoctors.Count == 0) continue;

                var derivationsInDept = deptGroup.ToList();
                
                // Distribuir las derivaciones entre los doctores del departamento
                for (int i = 0; i < derivationsInDept.Count; i++)
                {
                    var derivation = derivationsInDept[i];
                    var doctor = deptDoctors[i % deptDoctors.Count]; // Distribución round-robin
                    
                    // La consulta ocurre 1-3 días después de la derivación
                    var consultationDate = derivation.DateTimeDer.AddDays(random.Next(1, 4)).AddHours(random.Next(-2, 3));

                    consultations.Add(new ConsultationDerivation(
                        Guid.NewGuid(),
                        diagnoses[random.Next(diagnoses.Length)],
                        derivation.DerivationId,
                        consultationDate,
                        doctor.EmployeeId,
                        matchingHead.DepartmentHeadId
                    ));
                }
            }

            await _context.ConsultationDerivations.AddRangeAsync(consultations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {consultations.Count} consultas de derivación creadas (distribuidas entre {doctors.Count} doctores)");
        }
    }

    private async Task SeedConsultationReferralsAsync()
    {
        if (!await _context.ConsultationReferrals.AnyAsync())
        {
            var referrals = await _context.Referrals
                .Include(r => r.DepartmentTo)
                .ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Where(d => d.EmploymentStatus == EmploymentStatus.Active)
                .ToListAsync();
            var departmentHeads = await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .ToListAsync();

            if (referrals.Count == 0 || doctors.Count == 0 || departmentHeads.Count == 0) return;

            var consultations = new List<ConsultationReferral>();
            var diagnoses = new[]
            {
                "Paciente referido evaluado, condición requiere atención especializada",
                "Ingreso desde puesto externo procesado, se inicia tratamiento",
                "Evaluación de remisión completa, estabilización lograda",
                "Paciente de referencia presenta buena evolución",
                "Diagnóstico de remisión confirmado, seguimiento programado",
                "Atención de emergencia desde centro externo, paciente estabilizado",
                "Condición del paciente referido bajo control, alta programada",
                "Se confirma diagnóstico preliminar del puesto externo",
                "Requiere hospitalización para observación continua",
                "Tratamiento iniciado según protocolo de remisiones"
            };
            
            var random = new Random(321);

            // Agrupar remisiones por departamento destino
            var referralsByDept = referrals.GroupBy(r => r.DepartmentToId).ToList();

            foreach (var deptGroup in referralsByDept)
            {
                var deptId = deptGroup.Key;
                
                // Buscar el DepartmentHead del departamento destino
                var matchingHead = departmentHeads.FirstOrDefault(dh => dh.DepartmentId == deptId);
                if (matchingHead == null) continue;

                // Obtener doctores del departamento destino
                var deptDoctors = doctors.Where(d => d.DepartmentId == deptId).ToList();
                if (deptDoctors.Count == 0) continue;

                var referralsInDept = deptGroup.ToList();
                
                // Distribuir las remisiones entre los doctores del departamento
                for (int i = 0; i < referralsInDept.Count; i++)
                {
                    var referral = referralsInDept[i];
                    var doctor = deptDoctors[i % deptDoctors.Count]; // Distribución round-robin
                    
                    // La consulta ocurre el mismo día o 1-2 días después de la remisión
                    var consultationDate = referral.DateTimeRem.AddDays(random.Next(0, 3)).AddHours(random.Next(1, 5));

                    consultations.Add(new ConsultationReferral(
                        Guid.NewGuid(),
                        diagnoses[random.Next(diagnoses.Length)],
                        matchingHead.DepartmentHeadId,
                        doctor.EmployeeId,
                        referral.ReferralId,
                        consultationDate
                    ));
                }
            }

            await _context.ConsultationReferrals.AddRangeAsync(consultations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {consultations.Count} consultas de remisión creadas (distribuidas entre {doctors.Count} doctores)");
        }
    }

    private async Task SeedMedicationDerivationsAsync()
    {
        if (!await _context.MedicationDerivations.AnyAsync())
        {
            var consultationDerivations = await _context.ConsultationDerivations
                .Include(cd => cd.DepartmentHead)
                .ThenInclude(dh => dh!.Department)
                .ToListAsync();
            var medications = await _context.Medications.ToListAsync();
            var stockDepartments = await _context.StockDepartments.ToListAsync();

            if (consultationDerivations.Count == 0 || medications.Count == 0) return;

            var medDerivations = new List<MedicationDerivation>();
            var stockUpdates = new Dictionary<Guid, StockDepartment>(); // Para acumular cambios de stock

            foreach (var consultation in consultationDerivations)
            {
                var departmentId = consultation.DepartmentHead?.DepartmentId;
                if (departmentId == null) continue;

                // Asignar 1-2 medicamentos por consulta
                for (int i = 0; i < Math.Min(2, medications.Count); i++)
                {
                    var medication = medications[i];
                    var quantity = 10 + (i * 5); // Cantidad variada

                    // Buscar el stock del departamento para este medicamento
                    var stock = stockDepartments.FirstOrDefault(s => 
                        s.DepartmentId == departmentId && s.MedicationId == medication.MedicationId);
                    
                    if (stock != null && stock.Quantity >= quantity)
                    {
                        // Restar del stock (como lo hace el servicio)
                        stock.UpdateQuantity(stock.Quantity - quantity);
                        stockUpdates[stock.StockDepartmentId] = stock;

                        medDerivations.Add(new MedicationDerivation(
                            Guid.NewGuid(),
                            quantity,
                            consultation.ConsultationDerivationId,
                            medication.MedicationId
                        ));
                    }
                }
            }

            // Guardar medicamentos recetados
            await _context.MedicationDerivations.AddRangeAsync(medDerivations);
            
            // Actualizar stocks
            foreach (var stock in stockUpdates.Values)
            {
                _context.StockDepartments.Update(stock);
            }
            
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medDerivations.Count} medicamentos de derivación creados (stock actualizado)");
        }
    }

    private async Task SeedMedicationReferralsAsync()
    {
        if (!await _context.MedicationReferrals.AnyAsync())
        {
            var consultationReferrals = await _context.ConsultationReferrals
                .Include(cr => cr.DepartmentHead)
                .ThenInclude(dh => dh!.Department)
                .ToListAsync();
            var medications = await _context.Medications.ToListAsync();
            var stockDepartments = await _context.StockDepartments.ToListAsync();

            if (consultationReferrals.Count == 0 || medications.Count == 0) return;

            var medReferrals = new List<MedicationReferral>();
            var stockUpdates = new Dictionary<Guid, StockDepartment>();

            foreach (var consultation in consultationReferrals)
            {
                var departmentId = consultation.DepartmentHead?.DepartmentId;
                if (departmentId == null) continue;

                // Asignar 1-3 medicamentos por consulta
                for (int i = 0; i < Math.Min(3, medications.Count); i++)
                {
                    var medication = medications[i];
                    var quantity = 15 + (i * 7); // Cantidad variada

                    // Buscar el stock del departamento para este medicamento
                    var stock = stockDepartments.FirstOrDefault(s => 
                        s.DepartmentId == departmentId && s.MedicationId == medication.MedicationId);
                    
                    if (stock != null && stock.Quantity >= quantity)
                    {
                        // Restar del stock (como lo hace el servicio)
                        stock.UpdateQuantity(stock.Quantity - quantity);
                        stockUpdates[stock.StockDepartmentId] = stock;

                        medReferrals.Add(new MedicationReferral(
                            Guid.NewGuid(),
                            quantity,
                            consultation.ConsultationReferralId,
                            medication.MedicationId
                        ));
                    }
                }
            }

            // Guardar medicamentos recetados
            await _context.MedicationReferrals.AddRangeAsync(medReferrals);
            
            // Actualizar stocks
            foreach (var stock in stockUpdates.Values)
            {
                _context.StockDepartments.Update(stock);
            }
            
            await _context.SaveChangesAsync();
            Console.WriteLine($"✓ {medReferrals.Count} medicamentos de remisión creados (stock actualizado)");
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
                var status = (i % 5 - 2).ToString();
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

    /// <summary>
    /// Genera consultas de seguimiento para los 50 pacientes mayores (60-90 años).
    /// - 25 pacientes: segunda consulta en menos de 3 meses (mismo doctor)
    /// - 25 pacientes: segunda consulta en más de 3 meses (puede ser otro doctor)
    /// - Cada consulta incluye al menos 1 medicamento recetado
    /// </summary>
    private async Task SeedElderlyPatientFollowUpConsultationsAsync()
    {
        // Obtener pacientes mayores (identificación empieza con "50")
        var elderlyPatients = await _context.Patients
            .Where(p => p.Identification.StartsWith("50"))
            .ToListAsync();

        if (elderlyPatients.Count == 0)
        {
            Console.WriteLine("⚠ No se encontraron pacientes mayores para consultas de seguimiento");
            return;
        }

        var departments = await _context.Departments.ToListAsync();
        var doctors = await _context.Doctors
            .Where(d => d.EmploymentStatus == EmploymentStatus.Active)
            .ToListAsync();
        var departmentHeads = await _context.DepartmentHeads.ToListAsync();
        var medications = await _context.Medications.ToListAsync();
        var externalPosts = await _context.ExternalMedicalPosts.ToListAsync();
        var stockDepartments = await _context.StockDepartments.ToListAsync();

        if (departments.Count == 0 || doctors.Count == 0 || departmentHeads.Count == 0 || medications.Count == 0)
        {
            Console.WriteLine("⚠ Faltan datos necesarios para crear consultas de seguimiento");
            return;
        }

        var random = new Random(777);
        var elderlyDiagnoses = new[]
        {
            "Control de hipertensión arterial en adulto mayor",
            "Seguimiento de diabetes mellitus tipo 2",
            "Evaluación de función renal - paciente geriátrico",
            "Control de artritis reumatoide",
            "Seguimiento post-operatorio de fractura de cadera",
            "Evaluación de deterioro cognitivo leve",
            "Control de insuficiencia cardíaca congestiva",
            "Seguimiento de EPOC en adulto mayor",
            "Evaluación de osteoporosis y riesgo de fracturas",
            "Control de enfermedad de Parkinson"
        };

        var derivationsToCreate = new List<Derivation>();
        var referralsToCreate = new List<Referral>();
        var consultationDerivationsToCreate = new List<ConsultationDerivation>();
        var consultationReferralsToCreate = new List<ConsultationReferral>();
        var medicationDerivationsToCreate = new List<MedicationDerivation>();
        var medicationReferralsToCreate = new List<MedicationReferral>();
        var stockUpdates = new Dictionary<Guid, StockDepartment>();

        // Fecha base: 6 meses atrás para tener espacio para segundas consultas
        var baseDate = DateTime.UtcNow.AddMonths(-6);

        for (int i = 0; i < elderlyPatients.Count; i++)
        {
            var patient = elderlyPatients[i];
            var isEarlyFollowUp = i < 25; // Primera mitad: seguimiento temprano (<3 meses)
            
            // Seleccionar departamento aleatorio (preferir los nuevos departamentos geriátricos)
            var deptIndex = random.Next(6, departments.Count); // Índices 6-15 son los nuevos
            var department = departments[deptIndex];
            
            // Obtener doctores del departamento
            var deptDoctors = doctors.Where(d => d.DepartmentId == department.DepartmentId).ToList();
            if (deptDoctors.Count == 0) continue;
            
            var firstDoctor = deptDoctors[random.Next(deptDoctors.Count)];
            var departmentHead = departmentHeads.FirstOrDefault(dh => dh.DepartmentId == department.DepartmentId);
            if (departmentHead == null) continue;

            // ========== PRIMERA CONSULTA (tipo Derivación) ==========
            var firstConsultDate = baseDate.AddDays(random.Next(1, 30));
            
            // Crear derivación
            var derivation1 = new Derivation(
                Guid.NewGuid(),
                departments[2].DepartmentId, // Desde Medicina General
                firstConsultDate.AddHours(-2),
                patient.PatientId,
                department.DepartmentId
            );
            derivationsToCreate.Add(derivation1);

            // Crear consulta de derivación
            var consultation1 = new ConsultationDerivation(
                Guid.NewGuid(),
                $"Primera consulta: {elderlyDiagnoses[random.Next(elderlyDiagnoses.Length)]}",
                derivation1.DerivationId,
                firstConsultDate,
                firstDoctor.EmployeeId,
                departmentHead.DepartmentHeadId
            );
            consultationDerivationsToCreate.Add(consultation1);

            // Agregar medicamento(s) a la primera consulta (sin duplicados) - con actualización de stock
            var numMeds1 = random.Next(1, Math.Min(4, medications.Count + 1)); // 1-3 medicamentos
            var usedMedIds1 = new HashSet<Guid>();
            for (int m = 0; m < numMeds1; m++)
            {
                Guid medId;
                int attempts = 0;
                do
                {
                    medId = medications[random.Next(medications.Count)].MedicationId;
                    attempts++;
                } while (usedMedIds1.Contains(medId) && attempts < 10);
                
                if (!usedMedIds1.Contains(medId))
                {
                    usedMedIds1.Add(medId);
                    var quantity = random.Next(10, 60);
                    
                    // Buscar stock del departamento para este medicamento
                    var stock = stockDepartments.FirstOrDefault(s => 
                        s.MedicationId == medId && 
                        s.DepartmentId == department.DepartmentId);
                    
                    if (stock != null && stock.Quantity >= quantity)
                    {
                        // Actualizar cantidad en stock (como hace el servicio)
                        if (!stockUpdates.ContainsKey(stock.StockDepartmentId))
                        {
                            stockUpdates[stock.StockDepartmentId] = stock;
                        }
                        stock.UpdateQuantity(stock.Quantity - quantity);
                        
                        medicationDerivationsToCreate.Add(new MedicationDerivation(
                            Guid.NewGuid(),
                            quantity,
                            consultation1.ConsultationDerivationId,
                            medId
                        ));
                    }
                }
            }

            // ========== SEGUNDA CONSULTA (tipo Remisión) ==========
            DateTime secondConsultDate;
            Doctor secondDoctor;

            if (isEarlyFollowUp)
            {
                // Menos de 3 meses después, MISMO doctor
                var daysUntilSecond = random.Next(14, 85); // 2 semanas a ~2.8 meses
                secondConsultDate = firstConsultDate.AddDays(daysUntilSecond);
                secondDoctor = firstDoctor; // Mismo doctor
            }
            else
            {
                // Más de 3 meses después, puede ser OTRO doctor
                var daysUntilSecond = random.Next(92, 150); // 3+ meses
                secondConsultDate = firstConsultDate.AddDays(daysUntilSecond);
                // Puede ser otro doctor del mismo departamento
                secondDoctor = deptDoctors[random.Next(deptDoctors.Count)];
            }

            // Crear remisión para segunda consulta
            var referral2 = new Referral(
                Guid.NewGuid(),
                patient.PatientId,
                secondConsultDate.AddHours(-1),
                externalPosts[random.Next(externalPosts.Count)].ExternalMedicalPostId,
                department.DepartmentId
            );
            referralsToCreate.Add(referral2);

            // Crear consulta de remisión (segunda consulta)
            var followUpDiagnosis = isEarlyFollowUp 
                ? $"Seguimiento temprano (<3 meses): Paciente con buena evolución, continuar tratamiento"
                : $"Seguimiento tardío (>3 meses): {elderlyDiagnoses[random.Next(elderlyDiagnoses.Length)]}";

            var consultation2 = new ConsultationReferral(
                Guid.NewGuid(),
                followUpDiagnosis,
                departmentHead.DepartmentHeadId,
                secondDoctor.EmployeeId,
                referral2.ReferralId,
                secondConsultDate
            );
            consultationReferralsToCreate.Add(consultation2);

            // Agregar medicamento(s) a la segunda consulta (sin duplicados) - con actualización de stock
            var numMeds2 = random.Next(1, Math.Min(4, medications.Count + 1)); // 1-3 medicamentos
            var usedMedIds2 = new HashSet<Guid>();
            for (int m = 0; m < numMeds2; m++)
            {
                Guid medId;
                int attempts = 0;
                do
                {
                    medId = medications[random.Next(medications.Count)].MedicationId;
                    attempts++;
                } while (usedMedIds2.Contains(medId) && attempts < 10);
                
                if (!usedMedIds2.Contains(medId))
                {
                    usedMedIds2.Add(medId);
                    var quantity = random.Next(10, 60);
                    
                    // Buscar stock del departamento para este medicamento
                    var stock = stockDepartments.FirstOrDefault(s => 
                        s.MedicationId == medId && 
                        s.DepartmentId == department.DepartmentId);
                    
                    if (stock != null && stock.Quantity >= quantity)
                    {
                        // Actualizar cantidad en stock (como hace el servicio)
                        if (!stockUpdates.ContainsKey(stock.StockDepartmentId))
                        {
                            stockUpdates[stock.StockDepartmentId] = stock;
                        }
                        stock.UpdateQuantity(stock.Quantity - quantity);
                        
                        medicationReferralsToCreate.Add(new MedicationReferral(
                            Guid.NewGuid(),
                            quantity,
                            consultation2.ConsultationReferralId,
                            medId
                        ));
                    }
                }
            }
        }

        // Guardar todo en orden de dependencias
        await _context.Derivations.AddRangeAsync(derivationsToCreate);
        await _context.Referrals.AddRangeAsync(referralsToCreate);
        await _context.SaveChangesAsync();

        await _context.ConsultationDerivations.AddRangeAsync(consultationDerivationsToCreate);
        await _context.ConsultationReferrals.AddRangeAsync(consultationReferralsToCreate);
        await _context.SaveChangesAsync();

        await _context.MedicationDerivations.AddRangeAsync(medicationDerivationsToCreate);
        await _context.MedicationReferrals.AddRangeAsync(medicationReferralsToCreate);
        
        // Actualizar stocks de departamentos (como hace el servicio)
        _context.StockDepartments.UpdateRange(stockUpdates.Values);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✓ Consultas de seguimiento para pacientes mayores creadas:");
        Console.WriteLine($"  - {derivationsToCreate.Count} derivaciones (primera consulta)");
        Console.WriteLine($"  - {referralsToCreate.Count} remisiones (segunda consulta)");
        Console.WriteLine($"  - {consultationDerivationsToCreate.Count} consultas derivación");
        Console.WriteLine($"  - {consultationReferralsToCreate.Count} consultas remisión");
        Console.WriteLine($"  - {medicationDerivationsToCreate.Count} medicamentos en derivaciones");
        Console.WriteLine($"  - {medicationReferralsToCreate.Count} medicamentos en remisiones");
        Console.WriteLine($"  - {stockUpdates.Count} stocks de departamentos actualizados");
        Console.WriteLine($"  - 25 pacientes con seguimiento <3 meses (mismo doctor)");
        Console.WriteLine($"  - 25 pacientes con seguimiento >3 meses)");
    }
}
