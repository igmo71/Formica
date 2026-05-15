using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class ListWarehouses
{
    public static async Task<FeatureResult<IReadOnlyList<WarehouseEntity>>> HandleAsync(
        WarehouseDbContext dbContext,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Warehouses.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(warehouse => warehouse.IsActive);
        }

        var warehouses = await query
            .OrderBy(warehouse => warehouse.Code)
            .ToListAsync(cancellationToken);

        return FeatureResult<IReadOnlyList<WarehouseEntity>>.Success(warehouses);
    }
}
