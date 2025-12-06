using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PolyclinicInfrastructure.Persistence;
using PolyclinicInfrastructure.Repositories;
using PolyclinicInfrastructure.Identity;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Services.Implementations;
using PolyclinicCore.Constants;
using PolyclinicApplication.Mapping;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Logging;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;
using System.Text.Json.Serialization;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE SERVICIOS
// ==========================================

builder.Services.AddControllers()
    .AddJsonOptions(x => 
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();

// ==========================================
// SWAGGER con soporte para JWT
// ==========================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Polyclinic API",
        Version = "v1",
        Description = "API para gestión de policlínica con autenticación JWT"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese el token JWT. Swagger agregará automáticamente el prefijo 'Bearer '."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ==========================================
// INFRASTRUCTURE - DATABASE
// ==========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies() 
           .UseNpgsql( 
               builder.Configuration.GetConnectionString("DefaultConnection"),
               b => b.MigrationsAssembly("PolyclinicInfrastructure")));

// ==========================================
// INFRASTRUCTURE - IDENTITY
// ==========================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ==========================================
// INFRASTRUCTURE - JWT AUTHENTICATION
// ==========================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ Autenticación fallida: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
            Console.WriteLine($"✓ Token validado. Claims: {string.Join(", ", claims ?? Array.Empty<string>())}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ Challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// ==========================================
// INFRASTRUCTURE - AUTHORIZATION
// ==========================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole(ApplicationRoles.Admin));

    options.AddPolicy("RequireDoctorRole", policy =>
        policy.RequireRole(ApplicationRoles.Doctor));

    options.AddPolicy("RequireManagement", policy =>
        policy.RequireRole(
            ApplicationRoles.Admin,
            ApplicationRoles.DepartmentHead));
});

// ==========================================
// APPLICATION - MAPPING PROFILES
// ==========================================
builder.Services.AddAutoMapper(typeof(DepartmentProfile).Assembly);
builder.Services.AddAutoMapper(typeof(DoctorProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);
builder.Services.AddAutoMapper(typeof(DepartmentHeadProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PatientProfile).Assembly);
builder.Services.AddAutoMapper(typeof(DerivationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ReferralProfile).Assembly);
builder.Services.AddAutoMapper(typeof(NurseProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ConsultationReferralProfile).Assembly);

builder.Services.AddAutoMapper(typeof(MedicationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ConsultationDerivationProfile).Assembly);
// ==========================================
// APPLICATION - VALIDATION (FluentValidation)
// ==========================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Departments.CreateDepartmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateDoctorRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateDoctorRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Patients.CreatePatientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Derivations.CreateDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Referral.CreateReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateNurseRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateNurseRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateMedicationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateMedicationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Consultations.CreateConsultationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Consultations.UpdateConsultationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Auth.LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Auth.RegisterValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateConsultationDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateConsultationDerivationValidator>();
// ==========================================
// INFRASTRUCTURE - REPOSITORIES
// ==========================================
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDerivationRepository, DerivationRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
builder.Services.AddScoped<IConsultationReferralRepository, ConsultationReferralRepository>();
builder.Services.AddScoped<IPuestoExternoRepository,PuestoExternoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<INurseRepository, NurseRepository>();
builder.Services.AddScoped<IDepartmentHeadRepository, DepartmentHeadRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IConsultationDerivationRepository, ConsultationDerivationRepository>();
// Repositorio generico para empleados, definir para cada uno
builder.Services.AddScoped<IEmployeeRepository<Doctor>, DoctorRepository>();
builder.Services.AddScoped<IEmployeeRepository<Nurse>, NurseRepository>();

// ==========================================
// APPLICATION - SERVICES
// ==========================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IIdentityRepository, IdentityUserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IRoleValidationService, RoleValidationService>();
builder.Services.AddScoped<IEntityLinkingService, EntityLinkingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConsultationReferralService, ConsultationReferralService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDerivationService, DerivationService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INurseService, NurseService>();
builder.Services.AddScoped<IDepartmentHeadService, DepartmentHeadService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IConsultationDerivationService, ConsultationDerivationService>();
// Servico generico para empleados, definir para cada uno
builder.Services.AddScoped<IEmployeeService<DoctorResponse>, EmployeeService<Doctor, DoctorResponse>>();
builder.Services.AddScoped<IEmployeeService<NurseResponse>, EmployeeService<Nurse, NurseResponse>>();

var app = builder.Build();

// ==========================================
// INICIALIZACIÓN DE ROLES Y USUARIO ADMIN
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    foreach (var role in ApplicationRoles.AllRoles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var name = "Oscar";
    var identificationNumber = "1234567890";
    var age = 30;
    var contactNumber = "0987654321";
    var address = "Calle Falsa 123";
    var patient = await scope.ServiceProvider
        .GetRequiredService<IRepository<PolyclinicDomain.Entities.Patient>>()
        .FindAsync(p => p.Identification == identificationNumber);
    if (!patient.Any())
    {
        var newPatient = new PolyclinicDomain.Entities.Patient(
            Guid.NewGuid(),
            name,
            identificationNumber,
            age,
            contactNumber,
            address);
        await scope.ServiceProvider
            .GetRequiredService<IRepository<PolyclinicDomain.Entities.Patient>>()
            .AddAsync(newPatient);
        Console.WriteLine("✓ Paciente de prueba creado: Oscar");
    }
    else
    {
        Console.WriteLine("✓ Paciente de prueba ya existe: Oscar");
    }
    
    // ==========================================
    // SEEDING DE DATOS: ESCENARIO COMPLETO
    // ==========================================
    try 
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. Crear Departamento
        var deptName = "Cardiología";
        var department = await context.Set<Department>().FirstOrDefaultAsync(d => d.Name == deptName);
        if (department == null)
        {
            // Nota: Ajusta los parámetros del constructor según tu entidad Department
            department = new Department(Guid.NewGuid(), deptName);
            context.Add(department);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Departamento creado: {deptName}");
        }

        // 2. Crear Puesto Médico Externo
        var puestoName = "Centro de Salud Norte";
        var puesto = await context.Set<ExternalMedicalPost>().FirstOrDefaultAsync(p => p.Name == puestoName);
        if (puesto == null)
        {
            // Nota: Ajusta los parámetros según tu entidad ExternalMedicalPost
            puesto = new ExternalMedicalPost(Guid.NewGuid(), puestoName);
            context.Add(puesto);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Puesto Externo creado: {puestoName}");
        }

        // 3. Crear Doctor y Asignarlo al Departamento
        var docId = "DOC-CARDIO-01";
        var doctor = await context.Set<Doctor>().FirstOrDefaultAsync(d => d.Identification == docId);
        if (doctor == null)
        {
            // Nota: Ajusta los parámetros (Guid, Nombre, Apellido, Email, Tel, ID, Tarjeta, DeptId) según tu entidad Doctor
            doctor = new Doctor(
                Guid.NewGuid(), 
                "099123456", 
                "Roberto Gomez",
                EmploymentStatus.Active, 
                department.DepartmentId
            );
            context.Add(doctor);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Doctor creado y asignado a {deptName}: {doctor.Name}");
        }

        // 4. Asignar Doctor como Jefe de Departamento
        var head = await context.Set<DepartmentHead>().FirstOrDefaultAsync(dh => dh.DepartmentId == department.DepartmentId);
        if (head == null)
        {
            // Nota: Ajusta los parámetros según tu entidad DepartmentHead (Id, DoctorId, DeptId, Fecha)
            head = new DepartmentHead(Guid.NewGuid(), doctor.EmployeeId, department.DepartmentId, DateTime.UtcNow);
            context.Add(head);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Jefe de Departamento asignado: {doctor.Name} es jefe de {deptName}");
        }

        // 5. Crear Nuevo Paciente
        var patientId = "04080776679";
        var patientRef = await context.Set<Patient>().FirstOrDefaultAsync(p => p.Identification == patientId);
        if (patientRef == null)
        {
            patientRef = new Patient(
                Guid.NewGuid(), 
                "Maria Rodriguez", 
                patientId, 
                45, 
                "098765432", 
                "Calle La Paz 456"
            );
            context.Add(patientRef);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Paciente creado para remisión: Maria Rodriguez");
        }

        // 6. Crear Remisión (Puesto Externo -> Departamento)
        // Verificamos si ya existe una remisión hoy para este paciente a este departamento
        var existsReferral = await context.Set<Referral>().AnyAsync(r => 
            r.PatientId == patientRef.PatientId && 
            r.DepartmentTo!.DepartmentId == department.DepartmentId &&
            r.DateTimeRem.Date == DateTime.UtcNow.Date);

        if (!existsReferral)
        {
            var referral = new Referral(
                Guid.NewGuid(),
                patientRef.PatientId,
                DateTime.UtcNow,
                puesto.ExternalMedicalPostId,
                department.DepartmentId
            );
            context.Add(referral);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Remisión creada con éxito: Maria -> {deptName} (desde {puestoName})");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error durante el seeding de datos de prueba: {ex.Message}");
    }

    var adminEmail = "admin@polyclinic.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var defaultAdmin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(defaultAdmin, "Admin123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(defaultAdmin, ApplicationRoles.Admin);
            Console.WriteLine($"✓ Usuario administrador creado: {adminEmail}");
            Console.WriteLine("  Contraseña por defecto: Admin123!");
        }
        else
        {
            Console.WriteLine("✗ Error al crear usuario administrador:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  - {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine($"✓ Usuario administrador ya existe: {adminEmail}");
    }
}

// ==========================================
// MIDDLEWARE PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polyclinic API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();