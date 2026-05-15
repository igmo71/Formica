using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using ZoneEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones.Zone;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Zones;

public static class UpdateZone
{
    public sealed record Command(
        string? Code,
        string? Name,
        string? Purpose,
        string? Description);

    public static async Task<FeatureResult<ZoneEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Guid zoneId,
        Command command,
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

        if (!TryParsePurpose(command.Purpose, out var purpose, out var purposeFailure))
        {
            return FeatureResult<ZoneEntity>.ValidationFailed([purposeFailure]);
        }

        var validationResult = zone.TryUpdate(command.Code, command.Name, purpose, command.Description);
        if (!validationResult.IsValid)
        {
            return FeatureResult<ZoneEntity>.ValidationFailed(validationResult.Errors);
        }

        var codeInUse = await dbContext.Zones.AnyAsync(
            existing =>
                existing.Id != zoneId &&
                existing.WarehouseId == zone.WarehouseId &&
                existing.Code == zone.Code,
            cancellationToken);

        if (codeInUse)
        {
            return FeatureResult<ZoneEntity>.Conflict(
                "Zone.CodeNotUnique",
                $"Zone code '{zone.Code}' is already used in this warehouse.",
                "code");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<ZoneEntity>.Success(zone);
    }

    private static bool TryParsePurpose(
        string? purpose,
        out ZonePurpose parsedPurpose,
        out DomainValidationFailure failure)
    {
        parsedPurpose = default;
        failure = new("Zone.PurposeRequired", "Zone purpose is required.", "purpose");

        if (string.IsNullOrWhiteSpace(purpose))
        {
            return false;
        }

        if (!Enum.TryParse(purpose.Trim(), ignoreCase: true, out parsedPurpose) ||
            !Enum.IsDefined(parsedPurpose))
        {
            failure = new("Zone.PurposeInvalid", "Zone purpose is invalid.", "purpose");
            return false;
        }

        return true;
    }
}
