namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record UpdateWarehouseRequest(
    string? Code,
    string? Name,
    string? Description);
