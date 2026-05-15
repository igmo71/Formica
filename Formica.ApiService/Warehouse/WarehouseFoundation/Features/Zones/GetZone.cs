using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Zones;

public static class GetZone
{
    public static async Task<FeatureResult<ZoneEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid zoneId,
        CancellationToken cancellationToken)
    {
        var zone = await dbContext.Zones
            .AsNoTracking()
            .FirstOrDefaultAsync(zone => zone.Id == zoneId, cancellationToken);

        return zone is null
            ? FeatureResult<ZoneEntity>.NotFound(
                "Zone.NotFound",
                "Zone was not found.",
                "zoneId")
            : FeatureResult<ZoneEntity>.Success(zone);
    }
}
