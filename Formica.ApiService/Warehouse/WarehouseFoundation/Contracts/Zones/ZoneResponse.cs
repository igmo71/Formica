using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;

public sealed record ZoneResponse(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string Purpose,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc)
{
    public static ZoneResponse From(ZoneEntity zone)
        => new(
            zone.Id,
            zone.WarehouseId,
            zone.Code,
            zone.Name,
            zone.Purpose.ToString(),
            zone.Description,
            zone.IsActive,
            zone.CreatedAtUtc,
            zone.UpdatedAtUtc);
}
