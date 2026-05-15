using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class DeactivateWarehouse
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

        warehouse.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<WarehouseEntity>.Success(warehouse);
    }
}
