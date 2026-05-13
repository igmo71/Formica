using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Microsoft.EntityFrameworkCore;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class GetDefaultLocationAddressRules
{
    public static async Task<LocationAddressRulesFeatureResult> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
    {
        var warehouseExists = await dbContext.Warehouses.AnyAsync(
            warehouse => warehouse.Id == warehouseId,
            cancellationToken);

        if (!warehouseExists)
        {
            return new(WarehouseFeatureStatus.NotFound);
        }

        var rules = await dbContext.LocationAddressRules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                existing => existing.Code == LocationAddressRules.DefaultCode,
                cancellationToken);

        return rules is null
            ? new(WarehouseFeatureStatus.NotFound)
            : new(WarehouseFeatureStatus.Success, rules);
    }
}
