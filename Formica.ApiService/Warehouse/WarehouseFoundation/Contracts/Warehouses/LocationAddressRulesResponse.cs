namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record LocationAddressRulesResponse(
    int MaxLength,
    string? AllowedPattern,
    bool NormalizeToUppercase,
    bool TrimWhitespace,
    bool ZonePrefixRequired);
