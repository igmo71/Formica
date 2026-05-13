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
        => StorageLocationCapacity.Create(maxWeight, volume, height, width, depth);
}
