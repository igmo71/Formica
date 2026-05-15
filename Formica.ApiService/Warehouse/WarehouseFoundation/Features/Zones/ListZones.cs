using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Zones;

public static class ListZones
{
    public static async Task<FeatureResult<IReadOnlyList<ZoneEntity>>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid? warehouseId,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        if (warehouseId is null || warehouseId == Guid.Empty)
        {
            return FeatureResult<IReadOnlyList<ZoneEntity>>.ValidationFailed(
                [new DomainValidationFailure("Zone.WarehouseRequired", "Zone warehouse is required.", "warehouseId")]);
        }

        var query = dbContext.Zones
            .AsNoTracking()
            .Where(zone => zone.WarehouseId == warehouseId.Value);

        if (!includeInactive)
        {
            query = query.Where(zone => zone.IsActive);
        }

        var zones = await query
            .OrderBy(zone => zone.Code)
            .ToListAsync(cancellationToken);

        return FeatureResult<IReadOnlyList<ZoneEntity>>.Success(zones);
    }
}
