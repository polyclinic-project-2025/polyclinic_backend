using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class DerivationConfiguration : IEntityTypeConfiguration<Derivation>
{
    public void Configure(EntityTypeBuilder<Derivation> entity)
    {
        entity.ToTable("Derivation");

            entity.HasKey(d => d.DerivationId);
            
            entity.HasIndex(d => new { 
                    d.DepartmentFromId, 
                    d.DateTimeDer,
                    d.PatientId})
                    .IsUnique();

            entity.HasOne(d => d.DepartmentFrom)
                    .WithMany(d => d.DerivationsFrom)
                    .HasForeignKey(d => d.DepartmentFromId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Derivations)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(d => d.DepartmentTo)
                    .WithMany(d => d.DerivationsTo)
                    .HasForeignKey(d => d.DepartmentToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
    }
}