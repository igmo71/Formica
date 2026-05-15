using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.Persistence.Configurations.WarehouseFoundation;

public sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("zones", "warehouse_foundation");

        builder.HasKey(zone => zone.Id);

        builder.Property(zone => zone.Id)
            .ValueGeneratedNever();

        builder.Property(zone => zone.WarehouseId)
            .IsRequired();

        builder.HasOne<WarehouseEntity>()
            .WithMany()
            .HasForeignKey(zone => zone.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(zone => zone.Code)
            .HasMaxLength(Zone.MaxCodeLength)
            .IsRequired();

        builder.HasIndex(zone => new { zone.WarehouseId, zone.Code })
            .IsUnique();

        builder.Property(zone => zone.Name)
            .HasMaxLength(Zone.MaxNameLength)
            .IsRequired();

        builder.Property(zone => zone.Purpose)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(zone => zone.Description)
            .HasMaxLength(Zone.MaxDescriptionLength);

        builder.Property(zone => zone.IsActive)
            .IsRequired();

        builder.Property(zone => zone.CreatedAtUtc)
            .IsRequired();

        builder.Property(zone => zone.UpdatedAtUtc)
            .IsRequired();
    }
}
