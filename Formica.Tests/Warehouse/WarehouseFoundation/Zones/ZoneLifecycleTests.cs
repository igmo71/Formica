using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Zones;

public sealed class ZoneLifecycleTests
{
    [Fact]
    public void CreateValidZone()
    {
        var warehouseId = Guid.CreateVersion7();

        var validationResult = Zone.TryCreate(
            warehouseId,
            " storage ",
            " Main Storage ",
            ZonePurpose.Storage,
            " Main storage area ",
            out var zone);

        Assert.True(validationResult.IsValid);
        Assert.NotNull(zone);
        Assert.NotEqual(Guid.Empty, zone.Id);
        Assert.Equal(warehouseId, zone.WarehouseId);
        Assert.Equal("STORAGE", zone.Code);
        Assert.Equal("Main Storage", zone.Name);
        Assert.Equal(ZonePurpose.Storage, zone.Purpose);
        Assert.Equal("Main storage area", zone.Description);
        Assert.True(zone.IsActive);
        Assert.True(zone.CreatedAtUtc <= zone.UpdatedAtUtc);
    }

    [Fact]
    public void RejectBlankCode()
    {
        var validationResult = Zone.TryCreate(
            Guid.CreateVersion7(),
            " ",
            "Main Storage",
            ZonePurpose.Storage,
            null,
            out var zone);

        Assert.False(validationResult.IsValid);
        Assert.Null(zone);
        Assert.Contains(validationResult.Errors, error => error.Code == "Zone.CodeRequired");
    }

    [Fact]
    public void RejectBlankName()
    {
        var validationResult = Zone.TryCreate(
            Guid.CreateVersion7(),
            "STORAGE",
            " ",
            ZonePurpose.Storage,
            null,
            out var zone);

        Assert.False(validationResult.IsValid);
        Assert.Null(zone);
        Assert.Contains(validationResult.Errors, error => error.Code == "Zone.NameRequired");
    }

    [Fact]
    public void UpdateEditableAttributesPreservesIdentityAndWarehouse()
    {
        var warehouseId = Guid.CreateVersion7();
        var zone = CreateZone(warehouseId);
        var id = zone.Id;
        var createdAtUtc = zone.CreatedAtUtc;

        zone.Update(" picking ", " Picking Area ", ZonePurpose.Picking, " Fast pick ");

        Assert.Equal(id, zone.Id);
        Assert.Equal(warehouseId, zone.WarehouseId);
        Assert.Equal(createdAtUtc, zone.CreatedAtUtc);
        Assert.Equal("PICKING", zone.Code);
        Assert.Equal("Picking Area", zone.Name);
        Assert.Equal(ZonePurpose.Picking, zone.Purpose);
        Assert.Equal("Fast pick", zone.Description);
        Assert.True(zone.UpdatedAtUtc >= createdAtUtc);
    }

    [Fact]
    public void DeactivateAndReactivatePreservesIdentityAndChangesActiveState()
    {
        var zone = CreateZone(Guid.CreateVersion7());
        var id = zone.Id;

        zone.Deactivate();

        Assert.Equal(id, zone.Id);
        Assert.False(zone.IsActive);

        zone.Reactivate();

        Assert.Equal(id, zone.Id);
        Assert.True(zone.IsActive);
    }

    private static Zone CreateZone(Guid warehouseId)
    {
        var validationResult = Zone.TryCreate(
            warehouseId,
            "STORAGE",
            "Main Storage",
            ZonePurpose.Storage,
            null,
            out var zone);

        Assert.True(validationResult.IsValid);
        Assert.NotNull(zone);

        return zone;
    }
}
