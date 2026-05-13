using Formica.ApiService.Warehouse.Persistence;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class CreateWarehouse
{
    public sealed record Command(string? Code, string? Name, string? Description);

    public static async Task<WarehouseFeatureResult> HandleAsync(
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
            return new(WarehouseFeatureStatus.ValidationFailed, ValidationResult: validationResult);
        }

        if (await dbContext.Warehouses.AnyAsync(
            existing => existing.Code == warehouse.Code,
            cancellationToken))
        {
            return new(
                WarehouseFeatureStatus.Conflict,
                ConflictMessage: $"Warehouse code '{warehouse.Code}' is already used.",
                ConflictCode: "Warehouse.CodeNotUnique");
        }

        dbContext.Warehouses.Add(warehouse);

        if (!await dbContext.LocationAddressRules.AnyAsync(
            rules => rules.Code == LocationAddressRules.DefaultCode,
            cancellationToken))
        {
            dbContext.LocationAddressRules.Add(LocationAddressRules.CreateDefault());
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new(WarehouseFeatureStatus.Success, warehouse);
    }
}
