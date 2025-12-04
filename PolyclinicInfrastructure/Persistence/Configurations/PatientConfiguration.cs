using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;
using Microsoft.AspNetCore.Identity;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> entity)
    {
        entity.ToTable("Patient");

        entity.HasKey(e => e.PatientId);

        entity.HasIndex(e => e.Identification)
                .IsUnique();
        
        entity.HasOne<IdentityUser>()
        .WithOne()
        .HasForeignKey<Patient>(e => e.UserId)
        .OnDelete(DeleteBehavior.SetNull);
        
        entity.HasIndex(e => e.UserId)
                .IsUnique();
    }
}