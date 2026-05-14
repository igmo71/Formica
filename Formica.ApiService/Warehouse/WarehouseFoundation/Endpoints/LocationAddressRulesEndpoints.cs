using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.LocationAddressRules;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class LocationAddressRulesEndpoints
{
    public static RouteGroupBuilder MapLocationAddressRulesEndpoints(this RouteGroupBuilder group)
    {
        var rules = group.MapGroup("/location-address-rules")
            .WithTags("Location Address Rules");

        rules.MapGet("/default", GetDefaultAsync)
            .WithName("GetDefaultLocationAddressRules");

        return group;
    }

    private static async Task<IResult> GetDefaultAsync(
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var rules = await GetDefaultLocationAddressRules.HandleAsync(dbContext, cancellationToken);

        return rules is null
            ? EndpointResults.NotFound()
            : TypedResults.Ok(ToResponse(rules));
    }

    private static LocationAddressRulesResponse ToResponse(LocationAddressRules rules)
        => new(
            rules.MaxLength,
            rules.AllowedPattern,
            rules.NormalizeToUppercase,
            rules.TrimWhitespace,
            rules.ZonePrefixRequired);
}
