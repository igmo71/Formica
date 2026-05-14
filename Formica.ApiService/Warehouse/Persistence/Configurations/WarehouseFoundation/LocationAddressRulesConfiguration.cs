using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Formica.ApiService.Warehouse.Persistence.Configurations.WarehouseFoundation;

public sealed class LocationAddressRulesConfiguration : IEntityTypeConfiguration<LocationAddressRules>
{
    public void Configure(EntityTypeBuilder<LocationAddressRules> builder)
    {
        builder.ToTable("location_address_rules", "warehouse_foundation");

        builder.HasKey(rules => rules.Id);

        builder.Property(rules => rules.Id)
            .ValueGeneratedNever();

        builder.Property(rules => rules.Code)
            .HasMaxLength(LocationAddressRules.MaxCodeLength)
            .IsRequired();

        builder.HasIndex(rules => rules.Code)
            .IsUnique();

        builder.Property(rules => rules.MaxLength)
            .IsRequired();

        builder.Property(rules => rules.AllowedPattern)
            .HasMaxLength(LocationAddressRules.MaxAllowedPatternLength);

        builder.Property(rules => rules.NormalizeToUppercase)
            .IsRequired();

        builder.Property(rules => rules.TrimWhitespace)
            .IsRequired();

        builder.Property(rules => rules.ZonePrefixRequired)
            .IsRequired();

        builder.Property(rules => rules.IsActive)
            .IsRequired();

        builder.Property(rules => rules.CreatedAtUtc)
            .IsRequired();

        builder.Property(rules => rules.UpdatedAtUtc)
            .IsRequired();
    }
}
