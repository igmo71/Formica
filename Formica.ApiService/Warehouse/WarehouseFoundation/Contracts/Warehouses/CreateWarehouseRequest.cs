namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record CreateWarehouseRequest(
    string? Code,
    string? Name,
    string? Description);
