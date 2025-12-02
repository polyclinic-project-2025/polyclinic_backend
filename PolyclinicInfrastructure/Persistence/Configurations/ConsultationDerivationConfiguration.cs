using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class ConsultationDerivationConfiguration : IEntityTypeConfiguration<ConsultationDerivation>
{
    public void Configure(EntityTypeBuilder<ConsultationDerivation> entity)
    {
        entity.ToTable("ConsultationDerivation");

        entity.HasKey(cd => cd.ConsultationDerivationId);
        
        entity.HasIndex(c => new {
                c.DoctorId,
                c.DerivationId,
                c.DateTimeCDer})
                .IsUnique();

        entity.HasOne(c => c.DepartmentHead)
                .WithMany(dh => dh.ConsultationDerivations)
                .HasForeignKey(c => c.DepartmentHeadId)
                .HasPrincipalKey(dh => dh.DepartmentHeadId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        entity.HasOne(c => c.Doctor)
                .WithMany(d => d.ConsultationDerivations)
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        entity.HasOne(c => c.Derivation)
                .WithMany(d => d.ConsultationDerivations)
                .HasForeignKey(c => c.DerivationId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
    }
}