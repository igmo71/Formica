using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class GetWarehouse
{
    public static async Task<FeatureResult<WarehouseEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(warehouse => warehouse.Id == warehouseId, cancellationToken);

        return warehouse is null
            ? FeatureResult<WarehouseEntity>.NotFound(
                "Warehouse.NotFound",
                "Warehouse was not found.",
                "warehouseId")
            : FeatureResult<WarehouseEntity>.Success(warehouse);
    }
}
