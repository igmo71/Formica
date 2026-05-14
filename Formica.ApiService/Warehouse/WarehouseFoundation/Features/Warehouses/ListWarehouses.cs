using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class ListWarehouses
{
    public static Task<List<WarehouseEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Warehouses.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(warehouse => warehouse.IsActive);
        }

        return query
            .OrderBy(warehouse => warehouse.Code)
            .ToListAsync(cancellationToken);
    }
}
