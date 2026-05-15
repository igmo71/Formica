namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;

public sealed record CreateZoneRequest(
    Guid WarehouseId,
    string? Code,
    string? Name,
    string? Purpose,
    string? Description);
