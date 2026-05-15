namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;

public sealed record UpdateZoneRequest(
    string? Code,
    string? Name,
    string? Purpose,
    string? Description);
