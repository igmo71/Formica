using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public sealed record LocationAddressRulesFeatureResult(
    WarehouseFeatureStatus Status,
    LocationAddressRules? Rules = null);
