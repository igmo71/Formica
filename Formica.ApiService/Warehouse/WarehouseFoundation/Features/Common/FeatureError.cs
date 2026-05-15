namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;

public sealed record FeatureError(
    string Code,
    string Message,
    string? Field = null);
