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
using PolyclinicInfrastructure.Export;
using PolyclinicInfrastructure.Queries;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Services.Interfaces.Analytics;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.Services.Implementations.Analytics;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicCore.Constants;
using PolyclinicApplication.Mapping;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Logging;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;
using System.Text.Json.Serialization;
using PolyclinicApplication.Mappings;

IdentityModelEventSource.ShowPII = true;
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE SERVICIOS
// ==========================================

builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new PolyclinicAPI.Filters.ValidationFilter());
    })
    .AddJsonOptions(x => 
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();

// ==========================================
// SEGURIDAD - HSTS
// ==========================================
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

// Homogeneizar manejo de errores de validación (evitar ProblemDetails por defecto)
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

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
    options.RequireHttpsMetadata = true; // ✓ HTTPS obligatorio para seguridad
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
builder.Services.AddAutoMapper(typeof(WarehouseManagerProfile).Assembly);
builder.Services.AddAutoMapper(typeof(WarehouseRequestProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MedicationRequestProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MedicationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MedicationReferralProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ConsultationDerivationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MedicationDerivationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UnifiedConsultationProfile).Assembly);
builder.Services.AddAutoMapper(typeof(StockDepartmentProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EmergencyRoomProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EmergencyRoomCareProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MedicationEmergencyProfile).Assembly);
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
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.MedicationReferrals.CreateMedicationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.MedicationReferrals.UpdateMedicationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Consultations.CreateConsultationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Consultations.UpdateConsultationReferralValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Auth.LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Auth.RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateConsultationDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateConsultationDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateWarehouseManagerRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateWarehouseManagerRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.MedicationDerivation.CreateMedicationDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.MedicationDerivation.UpdateMedicationDerivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.StockDepartment.CreateStockDepartmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.StockDepartment.UpdateStockDepartmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateMedicationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateMedicationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateEmergencyRoomValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateEmergencyRoomCareValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.CreateMedicationEmergencyValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateEmergencyRoomValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateEmergencyRoomCareValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.UpdateMedicationEmergencyValidator>();

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
builder.Services.AddScoped<IWarehouseManagerRepository, WarehouseManagerRepository>();
builder.Services.AddScoped<IWarehouseRequestRepository, WarehouseRequestRepository>();
builder.Services.AddScoped<IMedicationRequestRepository, MedicationRequestRepository>();
builder.Services.AddScoped<IDepartmentHeadRepository, DepartmentHeadRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IMedicationReferralRepository, MedicationReferralRepository>();
builder.Services.AddScoped<IMedicationDerivationRepository, MedicationDerivationRepository>();
builder.Services.AddScoped<IConsultationDerivationRepository, ConsultationDerivationRepository>();
builder.Services.AddScoped<IStockDepartmentRepository, StockDepartmentRepository>();
builder.Services.AddScoped<IEmergencyRoomRepository, EmergencyRoomRepository>();
builder.Services.AddScoped<IEmergencyRoomCareRepository, EmergencyRoomCareRepository>();
builder.Services.AddScoped<IMedicationEmergencyRepository, MedicationEmergencyRepository>();

// Repositorio generico para empleados, definir para cada uno
builder.Services.AddScoped<IEmployeeRepository<Doctor>, DoctorRepository>();
builder.Services.AddScoped<IEmployeeRepository<Nurse>, NurseRepository>();
builder.Services.AddScoped<IEmployeeRepository<WarehouseManager>, WarehouseManagerRepository>();
// Repositorio de perfil de usuario (optimizado para obtener empleado o paciente vinculado)
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// ==========================================
// INFRASTRUCTURE - QUERIES
// ==========================================
builder.Services.AddScoped<IDeniedWarehouseRequestsQuery, DeniedWarehouseRequestsQuery>();
builder.Services.AddScoped<IDoctorMonthlyAverageQuery, DoctorMonthlyAverageQuery>();


// ==========================================
// APPLICATION - SERVICES
// ==========================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IIdentityRepository, IdentityUserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IRoleValidationService, RoleValidationService>();
builder.Services.AddScoped<IEntityLinkingService, EntityLinkingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IConsultationReferralService, ConsultationReferralService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDerivationService, DerivationService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INurseService, NurseService>();
builder.Services.AddScoped<IDepartmentHeadService, DepartmentHeadService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IMedicationReferralService, MedicationReferralService>();
builder.Services.AddScoped<IMedicationDerivationService, MedicationDerivationService>();
builder.Services.AddScoped<IConsultationDerivationService, ConsultationDerivationService>();
builder.Services.AddScoped<IWarehouseManagerService, WarehouseManagerService>();
builder.Services.AddScoped<IWarehouseRequestService, WarehouseRequestService>();
builder.Services.AddScoped<IMedicationRequestService, MedicationRequestService>();
builder.Services.AddScoped<IStockDepartmentService, StockDepartmentService>();
builder.Services.AddScoped<IUnifiedConsultationService, UnifiedConsultationService>();
builder.Services.AddScoped<IEmergencyRoomService, EmergencyRoomService>();
builder.Services.AddScoped<IEmergencyRoomCareService, EmergencyRoomCareService>();
builder.Services.AddScoped<IMedicationEmergencyService, MedicationEmergencyService>();
// Servico generico para empleados, definir para cada uno
builder.Services.AddScoped<IEmployeeService<DoctorResponse>, EmployeeService<Doctor, DoctorResponse>>();
builder.Services.AddScoped<IEmployeeService<WarehouseManagerResponse>, EmployeeService<WarehouseManager, WarehouseManagerResponse>>();
// Export services
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddSingleton<IExportStrategyFactory, ExportStrategyFactory>();
// Analytics
builder.Services.AddScoped<IDeniedWarehouseRequestsService, DeniedWarehouseRequestsService>();
builder.Services.AddScoped<IDoctorMonthlyAverageService, DoctorMonthlyAverageService>();



var app = builder.Build();

// ==========================================
// SEEDING DE BASE DE DATOS
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        var seeder = new DatabaseSeeder(context, userManager, roleManager);
        await seeder.SeedAsync();
        
        Console.WriteLine("✓ Inicialización de base de datos completada");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Ocurrió un error al sembrar la base de datos");
        throw;
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
else
{
    // En producción, forzar HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(policy => policy
    .WithOrigins(
        "http://localhost:3000",  // HTTP en desarrollo
        "https://localhost:3000"   // HTTPS en desarrollo
    )
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();