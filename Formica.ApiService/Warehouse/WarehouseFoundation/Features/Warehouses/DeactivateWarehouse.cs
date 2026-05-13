using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class DeactivateWarehouse
{
    public static async Task<WarehouseFeatureResult> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(existing => existing.Id == warehouseId, cancellationToken);

        if (warehouse is null)
        {
            return new(WarehouseFeatureStatus.NotFound);
        }

        warehouse.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(WarehouseFeatureStatus.Success, warehouse);
    }
}
