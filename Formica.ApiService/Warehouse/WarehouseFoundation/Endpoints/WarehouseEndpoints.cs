using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this RouteGroupBuilder group)
    {
        var warehouses = group.MapGroup("/warehouses")
            .WithTags("Warehouses");

        warehouses.MapGet("", ListAsync)
            .WithName("ListWarehouses");

        warehouses.MapGet("/{warehouseId:guid}", GetAsync)
            .WithName("GetWarehouse");

        warehouses.MapPost("", CreateAsync)
            .WithName("CreateWarehouse");

        warehouses.MapPut("/{warehouseId:guid}", UpdateAsync)
            .WithName("UpdateWarehouse");

        warehouses.MapPost("/{warehouseId:guid}/deactivate", DeactivateAsync)
            .WithName("DeactivateWarehouse");

        warehouses.MapPost("/{warehouseId:guid}/reactivate", ReactivateAsync)
            .WithName("ReactivateWarehouse");

        return group;
    }

    private static async Task<IResult> ListAsync(
        WarehouseDbContext dbContext,
        bool? includeInactive,
        CancellationToken cancellationToken)
    {
        var featureResult = await ListWarehouses.HandleAsync(dbContext, includeInactive == true, cancellationToken);

        var response = featureResult.Value is null
            ? Array.Empty<WarehouseResponse>()
            : featureResult.Value
                .Select(warehouse => WarehouseResponse.From(warehouse))
                .ToArray();

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var featureResult = await GetWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return featureResult.Status == FeatureResultStatus.Success && featureResult.Value is not null
            ? TypedResults.Ok(WarehouseResponse.From(featureResult.Value))
            : EndpointResults.NotFound();
    }

    private static async Task<IResult> CreateAsync(
        WarehouseDbContext dbContext,
        CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var featureResult = await CreateWarehouse.HandleAsync(
            dbContext,
            new(request.Code, request.Name, request.Description),
            cancellationToken);

        return ToWriteResult(featureResult, created: true);
    }

    private static async Task<IResult> UpdateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var featureResult = await UpdateWarehouse.HandleAsync(
            dbContext,
            warehouseId,
            new(request.Code, request.Name, request.Description),
            cancellationToken);

        return ToWriteResult(featureResult);
    }

    private static async Task<IResult> DeactivateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var featureResult = await DeactivateWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return ToWriteResult(featureResult);
    }

    private static async Task<IResult> ReactivateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var featureResult = await ReactivateWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return ToWriteResult(featureResult);
    }

    private static IResult ToWriteResult(FeatureResult<WarehouseEntity> featureResult, bool created = false)
    {
        return featureResult.Status switch
        {
            FeatureResultStatus.Success when featureResult.Value is not null && created
                => TypedResults.Created(
                    $"/api/warehouse-foundation/warehouses/{featureResult.Value.Id}",
                    WarehouseResponse.From(featureResult.Value)),
            FeatureResultStatus.Success when featureResult.Value is not null
                => TypedResults.Ok(WarehouseResponse.From(featureResult.Value)),
            FeatureResultStatus.ValidationFailed
                => EndpointResults.ValidationProblem(featureResult.ErrorList),
            FeatureResultStatus.Conflict
                => EndpointResults.Conflict(
                    featureResult.FirstError?.Message ?? "Warehouse request conflicts with existing data.",
                    featureResult.FirstError?.Code),
            FeatureResultStatus.NotFound => EndpointResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected warehouse feature result.")
        };
    }
}
