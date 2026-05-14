using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;
using LocationAddressRulesEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing.LocationAddressRules;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.LocationAddressRules;

public static class GetDefaultLocationAddressRules
{
    public static async Task<LocationAddressRulesEntity?> HandleAsync(
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken)
        => await dbContext.LocationAddressRules
            .AsNoTracking()
            .FirstOrDefaultAsync(
                rules => rules.Code == LocationAddressRulesEntity.DefaultCode,
                cancellationToken);
}
