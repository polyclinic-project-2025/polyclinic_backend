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


    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setting Employee
        modelBuilder.Entity<Employee>(entity =>
        {
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
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

        });
        // Setting ExternalMedicalPost
        modelBuilder.Entity<ExternalMedicalPost>(entity =>
        {
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

    }
}