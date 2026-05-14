using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;

public sealed record LocationAddressRulesResponse(
    int MaxLength,
    string? AllowedPattern,
    bool NormalizeToUppercase,
    bool TrimWhitespace,
    bool ZonePrefixRequired)
{
    public static LocationAddressRulesResponse From(LocationAddressRules rules)
        => new(
            rules.MaxLength,
            rules.AllowedPattern,
            rules.NormalizeToUppercase,
            rules.TrimWhitespace,
            rules.ZonePrefixRequired);
}
