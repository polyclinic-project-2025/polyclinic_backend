using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets declaration (TABLES)
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<ExternalMedicalPost> ExternalMedicalPosts { get; set; }
    public DbSet<Nursing> Nursing { get; set; }
    public DbSet<Warehouse> Warehouse { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<MedicalStaff> MedicalStaffs { get; set; }
    public DbSet<EmergencyDepartmentCare> EmergencyDepartmentCares { get; set; }
    public DbSet<MedicationRequest> MedicationRequests { get; set; }
    public DbSet<WarehouseRequest> WarehouseRequests { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setting Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.EmploymentStatus)
                .IsRequired();
        });

        // Setting User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.Email);

            entity.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>();
        });

        // Setting Nurse
        modelBuilder.Entity<Nurse>(entity =>
        {
            entity.ToTable("Nurse");

            entity.HasOne<Employee>()
                .WithOne()
                .HasForeignKey<Nurse>(n => n.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Nursing)
                .WithMany(e => e.Nurses)
                .HasForeignKey(e => e.NursingId) 
                .OnDelete(DeleteBehavior.Restrict);

        }
        );
        // Setting Nursing
        modelBuilder.Entity<Nursing>(entity =>
        {
            entity.ToTable("Nursing");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Boss)
                .WithOne()
                .HasForeignKey<Nursing>(e => e.BossId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

        });
        // Setting ExternalMedicalPost
        modelBuilder.Entity<ExternalMedicalPost>(entity =>
        {
            entity.ToTable("ExternalMedicalPost");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Address)
                .HasMaxLength(400);

        });
        // Setting Warehouse
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouse");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Boss)
                .WithOne()
                .HasForeignKey<Warehouse>(e => e.BossId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

        });
        //Setting EmergencyRoom

        // ============================
        // Department
        // ============================
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(e => e.Boss)
                .WithOne()
                .HasForeignKey<Department>(e => e.BossId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.MedicalStaff)
                .WithOne(ms => ms.Department)
                .HasForeignKey(ms => ms.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================
        // MedicalStaff
        // ============================
        modelBuilder.Entity<MedicalStaff>(entity =>
        {
            entity.ToTable("MedicalStaff");
            entity.HasOne<Employee>()
                .WithOne()
                .HasForeignKey<MedicalStaff>(ms => ms.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.MedicalStaff)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================
        // EmergencyRoomCare
        // ============================
        modelBuilder.Entity<EmergencyRoomCare>(entity =>
        {
            entity.ToTable("EmergencyRoomCare");
            entity.HasKey(e => new { e.DoctorId, e.PacientId, e.CareDate, e.GuardDate });

            entity.Property(e => e.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.CareDate).IsRequired();
            entity.Property(e => e.GuardDate).IsRequired();

            entity.HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Pacient)
                .WithMany()
                .HasForeignKey(e => e.PacientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================
        // MedicationRequest
        // ============================
        modelBuilder.Entity<MedicationRequest>(entity =>
        {
            entity.ToTable("MedicationRequest");
            entity.HasKey(e => new { e.MedicationId, e.DepartmentId, e.RequestDate });

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Medication)
                .WithMany()
                .HasForeignKey(e => e.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.RequestDate).IsRequired();
            entity.Property(e => e.Quantity).IsRequired(false);
        });

        // ============================
        // WarehouseRequest
        // ============================
        modelBuilder.Entity<WarehouseRequest>(entity =>
        {
            entity.ToTable("WarehouseRequest");
            entity.HasKey(e => new { e.WarehouseId, e.DepartmentId, e.RequestDate });

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.RequestDate)
                .IsRequired();

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Boss)
                .WithMany()
                .HasForeignKey(e => e.BossId)
                .OnDelete(DeleteBehavior.Restrict);


            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================
        // Doctor
        // ============================
        modelBuilder.Entity<Doctor>(entity =>
        {   
            entity.ToTable("Doctor");
            entity.HasOne<MedicalStaff>()
                .WithOne()
                .HasForeignKey<Doctor>(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
        // ============================
        // Boss
        // ============================
        modelBuilder.Entity<Boss>(entity =>
        {
            entity.ToTable("Boss");
            entity.HasOne<Employee>()
                .WithOne()
                .HasForeignKey<Boss>(b => b.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }       
}