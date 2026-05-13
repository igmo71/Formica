namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record LocationAddressRulesResponse(
    Guid WarehouseId,
    int MaxLength,
    string? AllowedPattern,
    bool NormalizeToUppercase,
    bool TrimWhitespace,
    bool ZonePrefixRequired);
