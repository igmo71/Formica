using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.EntityFrameworkCore;
using LocationAddressRulesEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing.LocationAddressRules;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class CreateWarehouse
{
    public sealed record Command(string? Code, string? Name, string? Description);

    public static async Task<FeatureResult<WarehouseEntity>> HandleAsync(
        WarehouseDbContext dbContext,
        Command command,
        CancellationToken cancellationToken)
    {
        var validationResult = WarehouseEntity.TryCreate(
            command.Code,
            command.Name,
            command.Description,
            out var warehouse);

        if (!validationResult.IsValid || warehouse is null)
        {
            return FeatureResult<WarehouseEntity>.ValidationFailed(validationResult.Errors);
        }

        if (await dbContext.Warehouses.AnyAsync(
            existing => existing.Code == warehouse.Code,
            cancellationToken))
        {
            return FeatureResult<WarehouseEntity>.Conflict(
                "Warehouse.CodeNotUnique",
                $"Warehouse code '{warehouse.Code}' is already used.",
                "code");
        }

        dbContext.Warehouses.Add(warehouse);

        if (!await dbContext.LocationAddressRules.AnyAsync(
            rules => rules.Code == LocationAddressRulesEntity.DefaultCode,
            cancellationToken))
        {
            dbContext.LocationAddressRules.Add(LocationAddressRulesEntity.CreateDefault());
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return FeatureResult<WarehouseEntity>.Success(warehouse);
    }
}
