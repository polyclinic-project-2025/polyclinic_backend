using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class EmergencyRoomConfiguration : IEntityTypeConfiguration<EmergencyRoom>
{
    public void Configure(EntityTypeBuilder<EmergencyRoom> entity)
    {
        entity.ToTable("EmergencyRoom");

        entity.HasKey(e => e.EmergencyRoomId);

        entity.HasIndex(e => new { 
                e.DoctorId, 
                e.GuardDate})
                .IsUnique();

        entity.HasOne(e => e.Doctor)
            .WithMany(d => d.EmergencyRooms)
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}