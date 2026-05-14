using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class GetWarehouse
{
    public static Task<WarehouseEntity?> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        CancellationToken cancellationToken)
        => dbContext.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(warehouse => warehouse.Id == warehouseId, cancellationToken);
}
