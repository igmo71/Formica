using System.Net.Http.Json;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;
using Formica.Tests.Warehouse.WarehouseFoundation.TestData;
using Microsoft.EntityFrameworkCore;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Zones;

public sealed class ZoneApiTests(WarehouseFoundationApiFixture fixture) : IClassFixture<WarehouseFoundationApiFixture>
{
    [Fact]
    public async Task CreateZoneInExistingWarehouseReturnsCreatedZoneWithActiveState()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Zone Create Warehouse", cancellationToken);
        var code = WarehouseFoundationTestData.UniqueZoneCode();

        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(warehouse.Id, code, "Main Storage", "Storage", "Main storage area"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(created);
        Assert.Equal(warehouse.Id, created.WarehouseId);
        Assert.Equal(code, created.Code);
        Assert.Equal("Main Storage", created.Name);
        Assert.Equal("Storage", created.Purpose);
        Assert.Equal("Main storage area", created.Description);
        Assert.True(created.IsActive);
    }

    [Fact]
    public async Task CreateZoneForMissingWarehouseReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(
                Guid.CreateVersion7(),
                WarehouseFoundationTestData.UniqueZoneCode(),
                "Missing Warehouse Zone",
                "Storage",
                null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, createResponse.StatusCode);
    }

    [Fact]
    public async Task DuplicateZoneCodeInSameWarehouseIsRejected()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Duplicate Zone Warehouse", cancellationToken);
        var code = WarehouseFoundationTestData.UniqueZoneCode();

        var firstResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(warehouse.Id, code, "First Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(warehouse.Id, code.ToLowerInvariant(), "Duplicate Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task SameZoneCodeInDifferentWarehousesIsAllowed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstWarehouse = await CreateWarehouseAsync("First Same Code Warehouse", cancellationToken);
        var secondWarehouse = await CreateWarehouseAsync("Second Same Code Warehouse", cancellationToken);
        var code = WarehouseFoundationTestData.UniqueZoneCode();

        var firstResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(firstWarehouse.Id, code, "First Zone", "Storage", null),
            cancellationToken);
        var secondResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(secondWarehouse.Id, code, "Second Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
    }

    [Fact]
    public async Task ListZonesReturnsActiveRecordsByDefaultAndCanIncludeInactive()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("List Active Warehouse", cancellationToken);
        var active = await CreateZoneAsync(warehouse.Id, "Active Zone", cancellationToken);
        var inactive = await CreateZoneAsync(warehouse.Id, "Inactive Zone", cancellationToken);
        await DeactivateZoneAsync(inactive.Id, cancellationToken);

        var activeZones = await fixture.ApiClient.GetFromJsonAsync<ZoneResponse[]>(
            $"/api/warehouse-foundation/zones?warehouseId={warehouse.Id}",
            cancellationToken);
        var allZones = await fixture.ApiClient.GetFromJsonAsync<ZoneResponse[]>(
            $"/api/warehouse-foundation/zones?warehouseId={warehouse.Id}&includeInactive=true",
            cancellationToken);

        Assert.Contains(activeZones ?? [], zone => zone.Id == active.Id && zone.IsActive);
        Assert.DoesNotContain(activeZones ?? [], zone => zone.Id == inactive.Id);
        Assert.Contains(allZones ?? [], zone => zone.Id == active.Id && zone.IsActive);
        Assert.Contains(allZones ?? [], zone => zone.Id == inactive.Id && !zone.IsActive);
    }

    [Fact]
    public async Task ListZonesUsesWarehouseIdFilter()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstWarehouse = await CreateWarehouseAsync("Filtered Zone Warehouse", cancellationToken);
        var secondWarehouse = await CreateWarehouseAsync("Other Zone Warehouse", cancellationToken);
        var firstZone = await CreateZoneAsync(firstWarehouse.Id, "Filtered Zone", cancellationToken);
        var otherZone = await CreateZoneAsync(secondWarehouse.Id, "Other Zone", cancellationToken);

        var zones = await fixture.ApiClient.GetFromJsonAsync<ZoneResponse[]>(
            $"/api/warehouse-foundation/zones?warehouseId={firstWarehouse.Id}",
            cancellationToken);

        Assert.Contains(zones ?? [], zone => zone.Id == firstZone.Id);
        Assert.DoesNotContain(zones ?? [], zone => zone.Id == otherZone.Id);
    }

    [Fact]
    public async Task GetZoneByIdReturnsExistingZone()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Get Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Get Zone", cancellationToken);

        var getResponse = await fixture.ApiClient.GetAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(fetched);
        Assert.Equal(zone.Id, fetched.Id);
        Assert.True(fetched.IsActive);
    }

    [Fact]
    public async Task GetZoneByIdReturnsInactiveZoneWithInactiveState()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Get Inactive Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Get Inactive Zone", cancellationToken);
        await DeactivateZoneAsync(zone.Id, cancellationToken);

        var getResponse = await fixture.ApiClient.GetAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(fetched);
        Assert.Equal(zone.Id, fetched.Id);
        Assert.False(fetched.IsActive);
    }

    [Fact]
    public async Task GetMissingZoneReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var getResponse = await fixture.ApiClient.GetAsync(
            $"/api/warehouse-foundation/zones/{Guid.CreateVersion7()}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static CreateWarehouseRequest WarehouseRequest(string name)
        => new(WarehouseFoundationTestData.UniqueWarehouseCode(), name, null);

    private async Task<WarehouseResponse> CreateWarehouseAsync(string name, CancellationToken cancellationToken)
    {
        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            WarehouseRequest(name),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(created);

        return created;
    }

    private async Task<ZoneResponse> CreateZoneAsync(
        Guid warehouseId,
        string name,
        CancellationToken cancellationToken)
    {
        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(
                warehouseId,
                WarehouseFoundationTestData.UniqueZoneCode(),
                name,
                "Storage",
                null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(created);

        return created;
    }

    private async Task DeactivateZoneAsync(Guid zoneId, CancellationToken cancellationToken)
    {
        await using var dbContext = fixture.CreateDbContext();
        var zone = await dbContext.Zones.FirstAsync(zone => zone.Id == zoneId, cancellationToken);
        zone.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
