using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.StorageLocations;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;
using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.Tests.Warehouse.WarehouseFoundation.TestData;

public static class WarehouseFoundationTestData
{
    public static string UniqueWarehouseCode(string prefix = "WH")
        => $"{prefix}-{Guid.CreateVersion7():N}"[..WarehouseEntity.MaxCodeLength].ToUpperInvariant();

    public static WarehouseEntity Warehouse(
        string? code = null,
        string name = "Test Warehouse",
        string? description = "Warehouse test data")
    {
        var validationResult = WarehouseEntity.TryCreate(
            code ?? UniqueWarehouseCode(),
            name,
            description,
            out var warehouse);

        if (!validationResult.IsValid || warehouse is null)
        {
            throw new InvalidOperationException("Default warehouse test data is invalid.");
        }

        return warehouse;
    }

    public static string UniqueZoneCode(string prefix = "ZONE")
        => $"{prefix}-{Guid.CreateVersion7():N}"[..ZoneEntity.MaxCodeLength].ToUpperInvariant();

    public static ZoneEntity Zone(
        Guid warehouseId,
        string? code = null,
        string name = "Test Zone",
        ZonePurpose purpose = ZonePurpose.Storage,
        string? description = "Zone test data")
    {
        var validationResult = ZoneEntity.TryCreate(
            warehouseId,
            code ?? UniqueZoneCode(),
            name,
            purpose,
            description,
            out var zone);

        if (!validationResult.IsValid || zone is null)
        {
            throw new InvalidOperationException("Default zone test data is invalid.");
        }

        return zone;
    }

    public static LocationAddressRules DefaultAddressRules()
        => LocationAddressRules.CreateDefault();

    public static string LocationAddress(string value = "a-01-01")
        => DefaultAddressRules().NormalizeAddress(value);

    public static StorageLocationCapacity Capacity(
        decimal? maxWeight = 1000m,
        decimal? volume = 2.5m,
        decimal? height = 1m,
        decimal? width = 1m,
        decimal? depth = 2.5m)
    {
        var validationResult = StorageLocationCapacity.TryCreate(
            out var capacity,
            maxWeight,
            volume,
            height,
            width,
            depth);

        if (!validationResult.IsValid || capacity is null)
        {
            throw new InvalidOperationException("Default warehouse foundation test capacity data is invalid.");
        }

        return capacity;
    }
}
