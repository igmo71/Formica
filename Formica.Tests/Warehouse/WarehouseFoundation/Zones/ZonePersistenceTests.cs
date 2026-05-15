using Formica.Tests.Warehouse.WarehouseFoundation.TestData;
using Microsoft.EntityFrameworkCore;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Zones;

public sealed class ZonePersistenceTests(WarehousePersistenceFixture fixture) : IClassFixture<WarehousePersistenceFixture>
{
    [Fact]
    public async Task DuplicateZoneCodeInSameWarehouseIsRejectedByPersistence()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var code = WarehouseFoundationTestData.UniqueZoneCode();

        await using var dbContext = fixture.CreateDbContext();
        var warehouse = WarehouseFoundationTestData.Warehouse();
        dbContext.Warehouses.Add(warehouse);
        dbContext.Zones.Add(WarehouseFoundationTestData.Zone(warehouse.Id, code));
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Zones.Add(WarehouseFoundationTestData.Zone(warehouse.Id, code.ToLowerInvariant(), "Duplicate Zone"));

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await dbContext.SaveChangesAsync(cancellationToken));
    }

    [Fact]
    public async Task SameZoneCodeInDifferentWarehousesIsAllowed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var code = WarehouseFoundationTestData.UniqueZoneCode();

        await using var dbContext = fixture.CreateDbContext();
        var firstWarehouse = WarehouseFoundationTestData.Warehouse();
        var secondWarehouse = WarehouseFoundationTestData.Warehouse();
        dbContext.Warehouses.AddRange(firstWarehouse, secondWarehouse);
        dbContext.Zones.Add(WarehouseFoundationTestData.Zone(firstWarehouse.Id, code));
        dbContext.Zones.Add(WarehouseFoundationTestData.Zone(secondWarehouse.Id, code));

        await dbContext.SaveChangesAsync(cancellationToken);

        var persistedCount = await dbContext.Zones
            .CountAsync(zone => zone.Code == code, cancellationToken);

        Assert.Equal(2, persistedCount);
    }

    [Fact]
    public async Task ZoneRequiresPersistedWarehouseRelationship()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var dbContext = fixture.CreateDbContext();
        dbContext.Zones.Add(WarehouseFoundationTestData.Zone(Guid.CreateVersion7()));

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await dbContext.SaveChangesAsync(cancellationToken));
    }
}
