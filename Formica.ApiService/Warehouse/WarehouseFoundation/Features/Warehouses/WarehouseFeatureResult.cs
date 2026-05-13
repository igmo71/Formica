using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;
using WarehouseEntity = Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses.Warehouse;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Warehouses;

public sealed record WarehouseFeatureResult(
    WarehouseFeatureStatus Status,
    WarehouseEntity? Warehouse = null,
    DomainValidationResult? ValidationResult = null,
    string? ConflictMessage = null,
    string? ConflictCode = null);
