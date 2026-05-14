using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.Persistence.Configurations.WarehouseFoundation;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseEntity>
{
    public void Configure(EntityTypeBuilder<WarehouseEntity> builder)
    {
        builder.ToTable("warehouses", "warehouse_foundation");

        builder.HasKey(warehouse => warehouse.Id);

        builder.Property(warehouse => warehouse.Id)
            .ValueGeneratedNever();

        builder.Property(warehouse => warehouse.Code)
            .HasMaxLength(WarehouseEntity.MaxCodeLength)
            .IsRequired();

        builder.HasIndex(warehouse => warehouse.Code)
            .IsUnique();

        builder.Property(warehouse => warehouse.Name)
            .HasMaxLength(WarehouseEntity.MaxNameLength)
            .IsRequired();

        builder.Property(warehouse => warehouse.Description)
            .HasMaxLength(WarehouseEntity.MaxDescriptionLength);

        builder.Property(warehouse => warehouse.IsActive)
            .IsRequired();

        builder.Property(warehouse => warehouse.CreatedAtUtc)
            .IsRequired();

        builder.Property(warehouse => warehouse.UpdatedAtUtc)
            .IsRequired();
    }
}
