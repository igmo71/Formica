namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

public sealed record DomainValidationFailure(
    string Code,
    string Message,
    string? Field = null);
