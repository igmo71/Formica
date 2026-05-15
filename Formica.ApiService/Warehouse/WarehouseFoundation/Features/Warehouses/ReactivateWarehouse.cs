using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class ReactivateWarehouse
{
    public static async Task<FeatureResult<WarehouseEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
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

        var codeInUse = await dbContext.Warehouses.AnyAsync(
            existing => existing.Id != warehouseId && existing.Code == warehouse.Code,
            cancellationToken);

        if (codeInUse)
        {
            return FeatureResult<WarehouseEntity>.Conflict(
                "Warehouse.CodeNotUnique",
                $"Warehouse code '{warehouse.Code}' is already used.",
                "code");
        }

        warehouse.Reactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<WarehouseEntity>.Success(warehouse);
    }
}
