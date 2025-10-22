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
    public DbSet<EmergencyRoomCare> EmergencyRoomCares { get; set; }
    public DbSet<MedicationRequest> MedicationRequests { get; set; }
    public DbSet<WarehouseRequest> WarehouseRequests { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Boss> Bosses { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Derivation> Derivations { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<ConsultationDerivation> ConsultationDerivations { get; set; }
    public DbSet<ConsultationReferral> ConsultationReferrals { get; set; }
    public DbSet<MedicationDerivation> MedicationDerivations {get; set;}
    public DbSet<MedicationReferral> MedicationReferrals {get;set;}
    public DbSet<MedicationEmergency> MedicationEmergency {get;set;}
    public DbSet<StockDepartment> StockDepartments {get;set;}
    public DbSet<EmergencyRoom> EmergencyRooms { get; set; }


    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setting Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");

            entity.HasKey(p => p.PatientId);

            entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

            entity.HasIndex(p => p.Contact)
                    .IsUnique();

            entity.Property(p => p.Age)
                    .IsRequired();

            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Patient_Age", "\"Age\" >= 0 AND \"Age\" < 130");
            });

            entity.Property(p => p.Address)
                    .HasMaxLength(400);
        });

        // Setting Derivation
        modelBuilder.Entity<Derivation>(entity =>
        {
            entity.ToTable("Derivation");

            // PRIMARY KEY (ID_Dpt1, ID_Pac, DateTime_Der)
            entity.HasKey(d => new { d.DepartmentFromId, d.PatientId, d.DateTimeDer });

            // FKs
            entity.HasOne(d => d.Patient)
                    .WithMany(e => e.Derivations)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(d => d.DepartmentFrom)
                    .WithMany()
                    .HasForeignKey(d => d.DepartmentFromId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(d => d.DepartmentTo)
                    .WithMany()
                    .HasForeignKey(d => d.DepartmentToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
        });

        // Setting Referral
        modelBuilder.Entity<Referral>(entity =>
        {
            entity.ToTable("Referral");

            // PRIMARY KEY (ID_Ext, ID_Pac, DateTime_Rem)
            entity.HasKey(r => new { r.ExternalMedicalPostId, r.PatientId, r.DateTimeRem });

            // FKs
            entity.HasOne(r => r.Patient)
                    .WithMany(e => e.Referrals)
                    .HasForeignKey(r => r.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(r => r.DepartmentTo)
                    .WithMany()
                    .HasForeignKey(r => r.DepartmentToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(r => r.ExternalMedicalPost)
                    .WithMany()
                    .HasForeignKey(r => r.ExternalMedicalPostId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
        });
 
        // Setting ConsulationDerivaiton
        modelBuilder.Entity<ConsultationDerivation>(entity =>
        {
            entity.ToTable("Consultation Derivation");

            // PRIMARY KEY (ID_Doc, ID_Dpt2, ID_Pac, DateTime_Der, DateTime_CDer, ID_Dpt1)
            entity.HasKey(c => new
            {
                c.DoctorId,
                c.DepartmentToId,
                c.PatientId,
                c.DateTimeDer,
                c.DateTimeCDer,
                c.DepartmentFromId
            });

            entity.Property(c => c.Diagnosis)
                    .HasMaxLength(1000);

            // FKs
            entity.HasOne(c => c.Patient)
                    .WithMany(e => e.ConsultationDerivations)
                    .HasForeignKey(c => c.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.DepartmentFrom)
                    .WithMany()
                    .HasForeignKey(c => c.DepartmentFromId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.DepartmentTo)
                    .WithMany()
                    .HasForeignKey(c => c.DepartmentToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.Doctor)
                    .WithMany()
                    .HasForeignKey(c => c.DoctorId)
                    .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(c => c.Boss)
                    .WithMany()
                    .HasForeignKey(c => c.BossId)
                    .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasMany(c => c.MedDer)
                .WithOne(md => md.Consulta);
        });

        // Setting ConsultationRefferal
        modelBuilder.Entity<ConsultationReferral>(entity =>
        {
            entity.ToTable("Consultation Referral");

            // PRIMARY KEY (ID_Doc, ID_Ext, ID_Pac, DateTime_Rem, DateTime_CRem, ID_Dpt2, Diagnóstico_Rem)
            entity.HasKey(c => new
            {
                c.DoctorId,
                c.ExternalMedicalPostId,
                c.PatientId,
                c.DateTimeRem,
                c.DateTimeCRem,
                c.DepartmentToId,
                c.Diagnosis
            });

            entity.Property(c => c.Diagnosis)
                    .HasMaxLength(1000);

            // FKs
            entity.HasOne(c => c.Patient)
                    .WithMany(e => e.ConsultationReferrals)
                    .HasForeignKey(c => c.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.ExternalMedicalPost)
                    .WithMany()
                    .HasForeignKey(c => c.ExternalMedicalPostId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.DepartmentTo)
                    .WithMany()
                    .HasForeignKey(c => c.DepartmentToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.Doctor)
                    .WithMany()
                    .HasForeignKey(c => c.DoctorId)
                    .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(c => c.Boss)
                    .WithMany()
                    .HasForeignKey(c => c.BossId)
                    .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasMany(c => c.MedRem)
                .WithOne(mr => mr.Consulta);
        });

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

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(e => e.Boss)
                .WithMany()
                .HasForeignKey(e => e.BossId)
                .OnDelete(DeleteBehavior.Restrict);
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
        modelBuilder.Entity<EmergencyRoom>(entity =>
        {
            entity.ToTable("EmergencyRoom");
            entity.HasKey(e => new { e.DoctorId, e.GuardDate });

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.EmergencyRooms)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.GuardDate)
                .IsRequired();
        });

        // Medicine configuration
        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(m => m.IdMed);

            entity.Property(m => m.Format)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(m => m.CommercialName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(m => m.CommercialCompany)
                .HasMaxLength(150);

            entity.Property(m => m.ScientificName)
                .HasMaxLength(150);

            entity.Property(m => m.BatchNumber)
                .HasMaxLength(100);

            entity.Property(m => m.ExpirationDate)
                .IsRequired();

            entity.HasMany(m => m.ConsultationDer)
                .WithOne(md => md.Medication);
            
            entity.HasMany(m => m.ConsultationRem)
                .WithOne(mr => mr.Medication);
            
            entity.HasMany(m => m.Emergency)
                .WithOne(me => me.Medication);
            
            entity.HasMany(m => m.Stock)
                .WithOne(sd => sd.Medication);

        });

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
            
            entity.HasMany(d => d.Stock)
                .WithOne(sd => sd.Department);
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
            entity.HasKey(e => new { e.DoctorId, e.PatientId, e.CareDate, e.GuardDate });

            entity.Property(e => e.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(e => e.EmergencyRoom)
                .WithMany()
                .HasForeignKey(e => new { e.DoctorId, e.GuardDate })
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.CareDate).IsRequired();
            entity.Property(e => e.GuardDate).IsRequired();

            entity.HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.MedEmergency)
                .WithOne(me => me.Emergency);
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
        //Medication Derivation
        modelBuilder.Entity<MedicationDerivation>(entity =>
        {
            entity.HasKey(md => new{
                md.DepartmentToId,
                md.DepartmentFromId,
                md.PatientId,
                md.DateTimeDer,
                md.DateTimeCDer,
                md.DoctorId,
                md.IdMed
                });
            entity.HasOne(md=> md.Medication)
                .WithMany(m => m.ConsultationDer)
                .HasForeignKey(md => md.IdMed);
            
            entity.HasOne(md => md.Consulta)
                .WithMany(c => c.MedDer)
                .HasForeignKey(md => new{
                    md.DepartmentToId,
                    md.DepartmentFromId,
                    md.PatientId,
                    md.DateTimeDer,
                    md.DateTimeCDer,
                    md.DoctorId
                    });
        });
        //Medication Referral
        modelBuilder.Entity<MedicationReferral>(entity =>
        {
            entity.HasKey(mr => new{
                mr.DoctorId,
                mr.ExternalMedicalPostId,
                mr.PatientId,
                mr.DateTimeRem,
                mr.DateTimeCRem,
                mr.DepartmentToId,
                mr.Diagnosis,
                mr.IdMed
            });
            entity.HasOne(mr => mr.Medication)
                .WithMany(m => m.ConsultationRem)
                .HasForeignKey(mr => mr.IdMed);
            
            entity.HasOne(mr => mr.Consulta)
                .WithMany(c => c.MedRem)
                .HasForeignKey(mr => new{
                    mr.DoctorId,
                    mr.ExternalMedicalPostId,
                    mr.PatientId,
                    mr.DateTimeRem,
                    mr.DateTimeCRem,
                    mr.DepartmentToId,
                    mr.Diagnosis
                });
        });
        //Medication Emergency
        modelBuilder.Entity<MedicationEmergency>(entity =>
        {
            entity.HasKey(me => new{
                me.DoctorId,
                me.PatientId,
                me.CareDate,
                me.GuardDate,
                me.IdMed
            });
            
            entity.HasOne(me => me.Medication)
                .WithMany(m => m.Emergency)
                .HasForeignKey(me => me.IdMed);
            
            entity.HasOne(me => me.Emergency)
                .WithMany(e => e.MedEmergency)
                .HasForeignKey(me => new{
                    me.DoctorId,
                    me.PatientId,
                    me.CareDate,
                    me.GuardDate
                });
        });
        //Stock Department
        modelBuilder.Entity<StockDepartment>(entity =>
        {
            entity.HasKey(sd => new{sd.Id, sd.IdMed});
            
            entity.HasOne(sd => sd.Medication)
                .WithMany(m => m.Stock)
                .HasForeignKey(sd => sd.IdMed);
            
            entity.HasOne(sd => sd.Department)
                .WithMany(d => d.Stock)
                .HasForeignKey(sd => sd.Id);
        });
    }       
}