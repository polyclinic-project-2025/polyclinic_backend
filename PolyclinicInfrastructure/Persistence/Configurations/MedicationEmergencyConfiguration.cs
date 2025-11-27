using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class MedicationEmergencyConfiguration : IEntityTypeConfiguration<MedicationEmergency>
{
    public void Configure(EntityTypeBuilder<MedicationEmergency> entity)
    {
        entity.ToTable("MedicationEmergency");

        entity.HasKey(me => me.MedicationEmergencyId);

        entity.HasIndex(me => new {
            me.EmergencyRoomCareId,
            me.MedicationId})
            .IsUnique();

        entity.HasOne(me => me.EmergencyRoomCare)
            .WithMany(erc => erc.MedicationEmergencies)
            .HasForeignKey(me => me.EmergencyRoomCareId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(me => me.Medication)
            .WithMany(m => m.MedicationEmergencies)
            .HasForeignKey(me => me.MedicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}