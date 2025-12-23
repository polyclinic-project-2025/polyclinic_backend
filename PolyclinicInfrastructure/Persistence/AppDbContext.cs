using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Aplica todos los triggers y funciones de validación a la base de datos.
    /// Este método debe llamarse después de aplicar las migraciones.
    /// </summary>
    public async Task ApplyDatabaseTriggersAsync()
    {
        // Trigger para validar ConsultationReferral
        await Database.ExecuteSqlRawAsync(DatabaseTriggers.CreateConsultationReferralValidationFunction);
        await Database.ExecuteSqlRawAsync(DatabaseTriggers.CreateConsultationReferralValidationTrigger);

        // Trigger para validar ConsultationDerivation
        await Database.ExecuteSqlRawAsync(DatabaseTriggers.CreateConsultationDerivationValidationFunction);
        await Database.ExecuteSqlRawAsync(DatabaseTriggers.CreateConsultationDerivationValidationTrigger);
    }

    // DbSets declaration (TABLES)
    // Usando Table-Per-Type (TPT) - cada entidad Employee en su propia tabla
    public DbSet<ConsultationDerivation> ConsultationDerivations { get; set; }
    public DbSet<ConsultationReferral> ConsultationReferrals { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentHead> DepartmentHeads { get; set; }
    public DbSet<Derivation> Derivations { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<EmergencyRoom> EmergencyRooms { get; set; }
    public DbSet<EmergencyRoomCare> EmergencyRoomCares { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<ExternalMedicalPost> ExternalMedicalPosts { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<MedicationDerivation> MedicationDerivations { get; set; }
    public DbSet<MedicationEmergency> MedicationEmergency { get; set; }
    public DbSet<MedicationReferral> MedicationReferrals { get; set; }
    public DbSet<MedicationRequest> MedicationRequests { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<StockDepartment> StockDepartments { get; set; }
    public DbSet<WarehouseManager> WarehouseManagers { get; set; }
    public DbSet<WarehouseRequest> WarehouseRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable(name: "Users");
        });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Roles");
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
           entity.ToTable("RoleClaims");
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        }); 

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }       
}