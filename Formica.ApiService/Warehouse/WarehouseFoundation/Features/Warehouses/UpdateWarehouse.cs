using Formica.ApiService.Warehouse.Persistence;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public static class UpdateWarehouse
{
    public sealed record Command(string? Code, string? Name, string? Description);

    public static async Task<WarehouseFeatureResult> HandleAsync(
        WarehouseDbContext dbContext,
        Guid warehouseId,
        Command command,
        CancellationToken cancellationToken)
    {
        var warehouse = await dbContext.Warehouses
            .FirstOrDefaultAsync(existing => existing.Id == warehouseId, cancellationToken);

        if (warehouse is null)
        {
            return new(WarehouseFeatureStatus.NotFound);
        }

        var validationResult = WarehouseEntity.Validate(command.Code, command.Name, command.Description);
        if (!validationResult.IsValid)
        {
            return new(WarehouseFeatureStatus.ValidationFailed, ValidationResult: validationResult);
        }

        var normalizedCode = WarehouseEntity.NormalizeCode(command.Code);
        var codeInUse = await dbContext.Warehouses.AnyAsync(
            existing => existing.Id != warehouseId && existing.Code == normalizedCode,
            cancellationToken);

        if (codeInUse)
        {
            return new(
                WarehouseFeatureStatus.Conflict,
                ConflictMessage: $"Warehouse code '{normalizedCode}' is already used.",
                ConflictCode: "Warehouse.CodeNotUnique");
        }

        _ = warehouse.TryUpdate(command.Code, command.Name, command.Description);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(WarehouseFeatureStatus.Success, warehouse);
    }
}
