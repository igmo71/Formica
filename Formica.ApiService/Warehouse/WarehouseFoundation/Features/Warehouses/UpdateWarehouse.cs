using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class UpdateWarehouse
{
    public sealed record Command(string? Code, string? Name, string? Description);

    public static async Task<FeatureResult<WarehouseEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        Command command,
        CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(existing => existing.Id == warehouseId, cancellationToken);

        if (warehouse is null)
        {
            return FeatureResult<WarehouseEntity>.NotFound(
                "Warehouse.NotFound",
                "Warehouse was not found.",
                "warehouseId");
        }

        var validationResult = warehouse.TryUpdate(command.Code, command.Name, command.Description);
        if (!validationResult.IsValid)
        {
            return FeatureResult<WarehouseEntity>.ValidationFailed(validationResult.Errors);
        }

        var normalizedCode = warehouse.Code;
        var codeInUse = await dbContext.Warehouses.AnyAsync(
            existing => existing.Id != warehouseId && existing.Code == normalizedCode,
            cancellationToken);

        if (codeInUse)
        {
            return FeatureResult<WarehouseEntity>.Conflict(
                "Warehouse.CodeNotUnique",
                $"Warehouse code '{normalizedCode}' is already used.",
                "code");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<WarehouseEntity>.Success(warehouse);
    }
}
