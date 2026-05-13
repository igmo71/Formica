using Formica.Tests.Warehouse.WarehouseFoundation.TestData;
using Microsoft.EntityFrameworkCore;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Warehouses;

public sealed class WarehousePersistenceTests(WarehousePersistenceFixture fixture) : IClassFixture<WarehousePersistenceFixture>
{
    [Fact]
    public async Task WarehouseCodeHasDatabaseUniquenessConstraint()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var code = WarehouseFoundationTestData.UniqueWarehouseCode();

        await using var dbContext = fixture.CreateDbContext();
        dbContext.Warehouses.Add(WarehouseFoundationTestData.Warehouse(code));
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Warehouses.Add(WarehouseFoundationTestData.Warehouse(code.ToLowerInvariant(), "Duplicate Warehouse"));

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await dbContext.SaveChangesAsync(cancellationToken));
    }
}
