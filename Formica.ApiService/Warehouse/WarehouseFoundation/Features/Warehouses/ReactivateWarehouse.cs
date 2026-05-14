using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class ReactivateWarehouse
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

        var codeInUse = await dbContext.Warehouses.AnyAsync(
            existing => existing.Id != warehouseId && existing.Code == warehouse.Code,
            cancellationToken);

        if (codeInUse)
        {
            return new(
                WarehouseFeatureStatus.Conflict,
                ConflictMessage: $"Warehouse code '{warehouse.Code}' is already used.",
                ConflictCode: "Warehouse.CodeNotUnique");
        }

        warehouse.Reactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(WarehouseFeatureStatus.Success, warehouse);
    }
}
