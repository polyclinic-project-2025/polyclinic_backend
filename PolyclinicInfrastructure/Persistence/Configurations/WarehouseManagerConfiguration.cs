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
    }
}