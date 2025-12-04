using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class EmergencyRoomCareConfiguration : IEntityTypeConfiguration<EmergencyRoomCare>
{
    public void Configure(EntityTypeBuilder<EmergencyRoomCare> entity)
    {
        entity.ToTable("EmergencyRoomCare");

        entity.HasKey(e => e.EmergencyRoomCareId);

        entity.HasIndex(e => new {
                e.CareDate, 
                e.PatientId})
                .IsUnique();

        entity.Property(e => e.Diagnosis)
            .HasMaxLength(1000);

        entity.HasOne(e => e.EmergencyRoom)
            .WithMany(er => er.EmergencyRoomCares)
            .HasForeignKey(e => e.EmergencyRoomId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(e => e.Patient)
            .WithMany(p => p.EmergencyRoomCares)
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}