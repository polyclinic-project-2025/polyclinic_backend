using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class WarehouseRequestConfiguration : IEntityTypeConfiguration<WarehouseRequest>
{
    public void Configure(EntityTypeBuilder<WarehouseRequest> entity)
    {
        entity.ToTable("WarehouseRequest");

        entity.HasKey(wr => wr.WarehouseRequestId);

        entity.HasIndex(wr => new {
                wr.RequestDate,
                wr.DepartmentId})
                .IsUnique();

        entity.HasOne(wr => wr.Department)
            .WithMany(d => d.WarehouseRequests)
            .HasForeignKey(wr => wr.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(wr => wr.WarehouseManager)
            .WithMany(wm => wm.WarehouseRequests)
            .HasForeignKey(wr => wr.WarehouseManagerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}