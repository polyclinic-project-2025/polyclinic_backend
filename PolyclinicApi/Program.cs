
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
using Application.Services.Interfaces;
using PolyclinicApplication.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Application.Services.Implementations;
using Microsoft.IdentityModel.Logging;

IdentityModelEventSource.ShowPII = true; // ⚠️ Solo en desarrollo
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE SERVICIOS
// ==========================================

// Add services to the container.
builder.Services.AddControllers();
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
            new string[] {}  // Array vacío, no List<string>()
        }
    });
});

// ==========================================
// INFRASTRUCTURE - DATABASE
// ==========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("PolyclinicInfrastructure")));

// ==========================================
// INFRASTRUCTURE - IDENTITY
// ==========================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    
    // Configuración de bloqueo
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
    options.RequireHttpsMetadata = false; // En producción, cambiar a true
    options.IncludeErrorDetails = true;
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ ERROR DE AUTENTICACIÓN:");
            Console.WriteLine($"   Excepción: {context.Exception?.GetType().Name}");
            Console.WriteLine($"   Mensaje: {context.Exception?.Message}");
            
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"   Header presente: {(authHeader.Length > 20 ? authHeader.Substring(0, 20) + "..." : authHeader)}");
            }
            else
            {
                Console.WriteLine($"   ⚠️ No hay header Authorization");
            }
            
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = context.Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value);
            Console.WriteLine($"✅ Token validado - Usuario: {userId}, Roles: {string.Join(", ", roles ?? Array.Empty<string>())}");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"📨 Token recibido: {authHeader.Substring(0, Math.Min(50, authHeader.Length))}...");
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.FromSeconds(5),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier,
        ValidateActor = false,
        ValidateTokenReplay = false
    };
});

// ==========================================
// INFRASTRUCTURE - AUTHORIZATION
// ==========================================
builder.Services.AddAuthorization(options =>
{
    // Políticas basadas en roles
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole(ApplicationRoles.Admin));

    options.AddPolicy("RequireDoctorRole", policy =>
        policy.RequireRole(ApplicationRoles.Doctor));

    options.AddPolicy("RequireMedicalStaff", policy =>
        policy.RequireRole(
            ApplicationRoles.Doctor,
            ApplicationRoles.Nurse,
            ApplicationRoles.MedicalStaff));

    options.AddPolicy("RequireManagement", policy =>
        policy.RequireRole(
            ApplicationRoles.Admin,
            ApplicationRoles.DepartmentHead));
});

// ==========================================
// APPLICATION - MAPPING PROFILES
// ==========================================

// AutoMapper escaneará todos los perfiles en el ensamblado PolyclinicApplication
builder.Services.AddAutoMapper(typeof(DepartmentProfile).Assembly);

// ==========================================
// APPLICATION - VALIDATION (FluentValidation)
// ==========================================

// Esta sección habilita la validación automática de DTOs con FluentValidation.
// ASP.NET ejecutará los validadores correspondientes antes de los controladores.
// Todos los validadores del ensamblado PolyclinicApplication serán registrados automáticamente.
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PolyclinicApplication.Validators.Departments.CreateDepartmentValidator>();


// ==========================================
// INFRASTRUCTURE - REPOSITORIES
// ==========================================

builder.Services.AddScoped<IDepartmentHeadRepository, DepartmentHeadRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
// builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
// builder.Services.AddScoped<IMedicalStaffRepository, MedicalStaffRepository>();
// builder.Services.AddScoped<INurseRepository, NurseRepository>();
// builder.Services.AddScoped<INursingHeadRepository, NursingHeadRepository>();
// builder.Services.AddScoped<IWarehouseManagerRepository, WarehouseManagerRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// Repositorios específicos de empleados
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
// builder.Services.AddScoped<IMedicalStaffRepository, MedicalStaffRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<INurseRepository, NurseRepository>();
builder.Services.AddScoped<INursingHeadRepository, NursingHeadRepository>();
builder.Services.AddScoped<IDepartmentHeadRepository, DepartmentHeadRepository>();
builder.Services.AddScoped<IWarehouseManagerRepository, WarehouseManagerRepository>();
// builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();


// ==========================================
// APPLICATION - SERVICES
// ==========================================
// Servicios de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IIdentityRepository, IdentityUserRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IRoleValidationService, RoleValidationService>();
builder.Services.AddScoped<IEntityLinkingService, EntityLinkingService>();
builder.Services.AddScoped<IUserService, UserService>();

// Servicios de dominio
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
// Servicios de empleados
builder.Services.AddScoped<IMedicalStaffService, MedicalStaffService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INurseService, NurseService>();
builder.Services.AddScoped<INursingHeadService, NursingHeadService>();
builder.Services.AddScoped<IDepartmentHeadService, DepartmentHeadService>();
builder.Services.AddScoped<IWarehouseManagerService, WarehouseManagerService>();


var app = builder.Build();

// ==========================================
// INICIALIZACIÓN DE ROLES Y USUARIO ADMIN
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Crear roles si no existen
    foreach (var role in ApplicationRoles.AllRoles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    //Crear paciente por defecto para testing
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

    // Crear usuario administrador por defecto
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

        // Contraseña por defecto: Admin123!
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