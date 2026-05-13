namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record WarehouseResponse(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
