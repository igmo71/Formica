using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.StorageLocations;

namespace Formica.Tests.Warehouse.WarehouseFoundation.TestData;

public static class WarehouseFoundationTestData
{
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
