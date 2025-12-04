using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class MedicationRequestConfiguration : IEntityTypeConfiguration<MedicationRequest>
{
    public void Configure(EntityTypeBuilder<MedicationRequest> entity)
    {
        entity.ToTable("MedicationRequest");

        entity.HasKey(mr => mr.MedicationRequestId);

        entity.HasIndex(mr => new {
            mr.WarehouseRequestId,
            mr.MedicationId})
            .IsUnique();

        entity.HasOne(mr => mr.WarehouseRequest)
            .WithMany(wr => wr.MedicationRequests)
            .HasForeignKey(mr => mr.WarehouseRequestId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(mr => mr.Medication)
            .WithMany(m => m.MedicationRequests)
            .HasForeignKey(mr => mr.MedicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}