using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class WarehouseEndpoints
{
    public static RouteGroupBuilder MapWarehouseEndpoints(this RouteGroupBuilder group)
    {
        var warehouses = group.MapGroup("/warehouses")
            .WithTags("Warehouses");

        warehouses.MapGet("/", ListAsync)
            .WithName("ListWarehouses");

        warehouses.MapGet("/{warehouseId:guid}", GetAsync)
            .WithName("GetWarehouse");

        warehouses.MapPost("/", CreateAsync)
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
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var warehouses = await ListWarehouses.HandleAsync(dbContext, includeInactive, cancellationToken);

        return TypedResults.Ok(warehouses.Select(ToResponse));
    }

    private static async Task<IResult> GetAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouse = await GetWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return warehouse is null
            ? EndpointResults.NotFound()
            : TypedResults.Ok(ToResponse(warehouse));
    }

    private static async Task<IResult> CreateAsync(
        WarehouseDbContext dbContext,
        CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await CreateWarehouse.HandleAsync(
            dbContext,
            new(request.Code, request.Name, request.Description),
            cancellationToken);

        return ToWriteResult(result, created: true);
    }

    private static async Task<IResult> UpdateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await UpdateWarehouse.HandleAsync(
            dbContext,
            warehouseId,
            new(request.Code, request.Name, request.Description),
            cancellationToken);

        return ToWriteResult(result);
    }

    private static async Task<IResult> DeactivateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var result = await DeactivateWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return ToWriteResult(result);
    }

    private static async Task<IResult> ReactivateAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var result = await ReactivateWarehouse.HandleAsync(dbContext, warehouseId, cancellationToken);

        return ToWriteResult(result);
    }

    private static IResult ToWriteResult(WarehouseFeatureResult result, bool created = false)
    {
        return result.Status switch
        {
            WarehouseFeatureStatus.Success when result.Warehouse is not null && created
                => TypedResults.Created(
                    $"/api/warehouse-foundation/warehouses/{result.Warehouse.Id}",
                    ToResponse(result.Warehouse)),
            WarehouseFeatureStatus.Success when result.Warehouse is not null
                => TypedResults.Ok(ToResponse(result.Warehouse)),
            WarehouseFeatureStatus.ValidationFailed when result.ValidationResult is not null
                => EndpointResults.ValidationProblem(result.ValidationResult),
            WarehouseFeatureStatus.Conflict
                => EndpointResults.Conflict(
                    result.ConflictMessage ?? "Warehouse request conflicts with existing data.",
                    result.ConflictCode),
            WarehouseFeatureStatus.NotFound => EndpointResults.NotFound(),
            _ => throw new InvalidOperationException("Unexpected warehouse feature result.")
        };
    }

    private static WarehouseResponse ToResponse(WarehouseEntity warehouse)
        => new(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            warehouse.Description,
            warehouse.IsActive,
            warehouse.CreatedAtUtc,
            warehouse.UpdatedAtUtc);
}
