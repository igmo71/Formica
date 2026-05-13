namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Common;

public sealed record LifecycleDto(
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
