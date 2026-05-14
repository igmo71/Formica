using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

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
        var warehouses = await ListWarehouses.HandleAsync(dbContext, includeInactive == true, cancellationToken);

        var response = warehouses
        .Select(warehouse => WarehouseResponse.From(warehouse))
        .ToArray();

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouse = await GetWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return warehouse is null
            ? EndpointResults.NotFound()
            : TypedResults.Ok(WarehouseResponse.From(warehouse));
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

    private static IResult ToWriteResult(WarehouseFeatureResult featureResult, bool created = false)
    {
        return featureResult.Status switch
        {
            WarehouseFeatureStatus.Success when featureResult.Warehouse is not null && created
                => TypedResults.Created(
                    $"/api/warehouse-foundation/warehouses/{featureResult.Warehouse.Id}",
                    WarehouseResponse.From(featureResult.Warehouse)),
            WarehouseFeatureStatus.Success when featureResult.Warehouse is not null
                => TypedResults.Ok(WarehouseResponse.From(featureResult.Warehouse)),
            WarehouseFeatureStatus.ValidationFailed when featureResult.ValidationResult is not null
                => EndpointResults.ValidationProblem(featureResult.ValidationResult),
            WarehouseFeatureStatus.Conflict
                => EndpointResults.Conflict(
                    featureResult.ConflictMessage ?? "Warehouse request conflicts with existing data.",
                    featureResult.ConflictCode),
            WarehouseFeatureStatus.NotFound => EndpointResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected warehouse feature result.")
        };
    }
}
