using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class WarehouseManagerConfiguration : IEntityTypeConfiguration<WarehouseManager>
{
    public void Configure(EntityTypeBuilder<WarehouseManager> entity)
    {
        entity.ToTable("WarehouseManager");

        entity.HasOne(wm => wm.Warehouse)
                .WithOne(w => w.WarehouseManager)
                .HasForeignKey<WarehouseManager>(wm => wm.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
    }
}