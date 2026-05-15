using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Zones;

public static class DeactivateZone
{
    public static async Task<FeatureResult<ZoneEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid zoneId,
        CancellationToken cancellationToken)
    {
        var zone = await dbContext.Zones
            .FirstOrDefaultAsync(existing => existing.Id == zoneId, cancellationToken);

        if (zone is null)
        {
            return FeatureResult<ZoneEntity>.NotFound(
                "Zone.NotFound",
                "Zone was not found.",
                "zoneId");
        }

        zone.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<ZoneEntity>.Success(zone);
    }
}
