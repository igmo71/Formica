using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Zones;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class ZoneEndpoints
{
    public static RouteGroupBuilder MapZoneEndpoints(this RouteGroupBuilder group)
    {
        var zones = group.MapGroup("/zones")
            .WithTags("Zones");

        zones.MapGet("", ListAsync)
            .WithName("ListZones");

        zones.MapGet("/{zoneId:guid}", GetAsync)
            .WithName("GetZone");

        zones.MapPost("", CreateAsync)
            .WithName("CreateZone");

        return group;
    }

    private static async Task<IResult> ListAsync(
        WarehouseDbContext dbContext,
        Guid? warehouseId,
        bool? includeInactive,
        CancellationToken cancellationToken)
    {
        var featureResult = await ListZones.HandleAsync(
            dbContext,
            warehouseId,
            includeInactive == true,
            cancellationToken);

        if (featureResult.Status == FeatureResultStatus.ValidationFailed)
        {
            return EndpointResults.ValidationProblem(featureResult.ErrorList);
        }

        if (!featureResult.IsSuccess || featureResult.Value is null)
        {
            return TypedResults.Problem(
                title: "Unexpected zone list result.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var response = featureResult.Value
            .Select(zone => ZoneResponse.From(zone))
            .ToArray();

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetAsync(
        WarehouseDbContext dbContext,
        Guid zoneId,
        CancellationToken cancellationToken)
    {
        var featureResult = await GetZone.HandleAsync(dbContext, zoneId, cancellationToken);

        return featureResult.Status == FeatureResultStatus.Success && featureResult.Value is not null
            ? TypedResults.Ok(ZoneResponse.From(featureResult.Value))
            : EndpointResults.NotFound();
    }

    private static async Task<IResult> CreateAsync(
        WarehouseDbContext dbContext,
        CreateZoneRequest request,
        CancellationToken cancellationToken)
    {
        var featureResult = await CreateZone.HandleAsync(
            dbContext,
            new(request.WarehouseId, request.Code, request.Name, request.Purpose, request.Description),
            cancellationToken);

        return ToWriteResult(featureResult, created: true);
    }

    private static IResult ToWriteResult(FeatureResult<Zone> featureResult, bool created = false)
    {
        return featureResult.Status switch
        {
            FeatureResultStatus.Success when featureResult.Value is not null && created
                => TypedResults.Created(
                    $"/api/warehouse-foundation/zones/{featureResult.Value.Id}",
                    ZoneResponse.From(featureResult.Value)),
            FeatureResultStatus.Success when featureResult.Value is not null
                => TypedResults.Ok(ZoneResponse.From(featureResult.Value)),
            FeatureResultStatus.ValidationFailed
                => EndpointResults.ValidationProblem(featureResult.ErrorList),
            FeatureResultStatus.Conflict
                => EndpointResults.Conflict(
                    featureResult.FirstError?.Message ?? "Zone request conflicts with existing data.",
                    featureResult.FirstError?.Code),
            FeatureResultStatus.NotFound => EndpointResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected zone feature result.")
        };
    }
}
