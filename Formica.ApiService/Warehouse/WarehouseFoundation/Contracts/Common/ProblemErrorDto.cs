namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Common;

public sealed record ProblemErrorDto(
    string Code,
    string Message,
    string? Field = null);
