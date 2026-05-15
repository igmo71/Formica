using System.Net.Http.Json;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Zones;
using Formica.Tests.Warehouse.WarehouseFoundation.TestData;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Zones;

public sealed class ZoneLifecycleApiTests(WarehouseFoundationApiFixture fixture) : IClassFixture<WarehouseFoundationApiFixture>
{
    [Fact]
    public async Task UpdateZoneSuccessPreservesIdAndWarehouseId()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Update Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Original Zone", cancellationToken);
        var updatedCode = WarehouseFoundationTestData.UniqueZoneCode();

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}",
            new UpdateZoneRequest(updatedCode, "Updated Zone", "Picking", "Updated description"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(updated);
        Assert.Equal(zone.Id, updated.Id);
        Assert.Equal(warehouse.Id, updated.WarehouseId);
        Assert.Equal(updatedCode, updated.Code);
        Assert.Equal("Updated Zone", updated.Name);
        Assert.Equal("Picking", updated.Purpose);
        Assert.Equal("Updated description", updated.Description);
        Assert.True(updated.IsActive);
    }

    [Fact]
    public async Task UpdateZoneRejectsInvalidInputThroughValidationResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Invalid Update Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Invalid Update Zone", cancellationToken);

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}",
            new UpdateZoneRequest(" ", "Updated Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

        var getResponse = await fixture.ApiClient.GetAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}",
            cancellationToken);
        var current = await getResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);

        Assert.NotNull(current);
        Assert.Equal(zone.Code, current.Code);
        Assert.Equal(zone.Name, current.Name);
    }

    [Fact]
    public async Task UpdateZoneRejectsDuplicateCodeInSameWarehouse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Duplicate Update Zone Warehouse", cancellationToken);
        var existing = await CreateZoneAsync(warehouse.Id, "Existing Zone", cancellationToken);
        var target = await CreateZoneAsync(warehouse.Id, "Target Zone", cancellationToken);

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/zones/{target.Id}",
            new UpdateZoneRequest(existing.Code.ToLowerInvariant(), "Target Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, updateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateZoneAllowsSameCodeInAnotherWarehouse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstWarehouse = await CreateWarehouseAsync("First Cross Warehouse Update", cancellationToken);
        var secondWarehouse = await CreateWarehouseAsync("Second Cross Warehouse Update", cancellationToken);
        var firstZone = await CreateZoneAsync(firstWarehouse.Id, "First Cross Zone", cancellationToken);
        var secondZone = await CreateZoneAsync(secondWarehouse.Id, "Second Cross Zone", cancellationToken);

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/zones/{secondZone.Id}",
            new UpdateZoneRequest(firstZone.Code.ToLowerInvariant(), "Second Cross Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(updated);
        Assert.Equal(secondZone.Id, updated.Id);
        Assert.Equal(secondWarehouse.Id, updated.WarehouseId);
        Assert.Equal(firstZone.Code, updated.Code);
    }

    [Fact]
    public async Task UpdateMissingZoneReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/zones/{Guid.CreateVersion7()}",
            new UpdateZoneRequest(WarehouseFoundationTestData.UniqueZoneCode(), "Missing Zone", "Storage", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeactivateZoneReturnsInactiveStateAndPreservesIdentity()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Deactivate Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Deactivate Zone", cancellationToken);

        var deactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}/deactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);
        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(deactivated);
        Assert.Equal(zone.Id, deactivated.Id);
        Assert.Equal(warehouse.Id, deactivated.WarehouseId);
        Assert.False(deactivated.IsActive);
    }

    [Fact]
    public async Task DeactivateMissingZoneReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var deactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{Guid.CreateVersion7()}/deactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, deactivateResponse.StatusCode);
    }

    [Fact]
    public async Task ReactivateZoneReturnsActiveStateAndPreservesIdentity()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var warehouse = await CreateWarehouseAsync("Reactivate Zone Warehouse", cancellationToken);
        var zone = await CreateZoneAsync(warehouse.Id, "Reactivate Zone", cancellationToken);
        await DeactivateZoneAsync(zone.Id, cancellationToken);

        var reactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{zone.Id}/reactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, reactivateResponse.StatusCode);
        var reactivated = await reactivateResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(reactivated);
        Assert.Equal(zone.Id, reactivated.Id);
        Assert.Equal(warehouse.Id, reactivated.WarehouseId);
        Assert.True(reactivated.IsActive);
    }

    [Fact]
    public async Task ReactivateMissingZoneReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var reactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{Guid.CreateVersion7()}/reactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, reactivateResponse.StatusCode);
    }

    [Fact]
    public async Task ReactivateZoneUsesWarehouseScopedUniquenessCheck()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstWarehouse = await CreateWarehouseAsync("First Reactivate Scope Warehouse", cancellationToken);
        var secondWarehouse = await CreateWarehouseAsync("Second Reactivate Scope Warehouse", cancellationToken);
        var code = WarehouseFoundationTestData.UniqueZoneCode();
        var firstZone = await CreateZoneAsync(firstWarehouse.Id, "First Reactivate Scope Zone", cancellationToken, code);
        var secondZone = await CreateZoneAsync(secondWarehouse.Id, "Second Reactivate Scope Zone", cancellationToken, code);
        await DeactivateZoneAsync(firstZone.Id, cancellationToken);

        var reactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{firstZone.Id}/reactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, reactivateResponse.StatusCode);
        var reactivated = await reactivateResponse.Content.ReadFromJsonAsync<ZoneResponse>(cancellationToken);
        Assert.NotNull(reactivated);
        Assert.Equal(firstZone.Id, reactivated.Id);
        Assert.Equal(firstWarehouse.Id, reactivated.WarehouseId);
        Assert.Equal(secondZone.Code, reactivated.Code);
        Assert.True(reactivated.IsActive);
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
        CancellationToken cancellationToken,
        string? code = null)
    {
        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/zones",
            new CreateZoneRequest(
                warehouseId,
                code ?? WarehouseFoundationTestData.UniqueZoneCode(),
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
        var deactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/zones/{zoneId}/deactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);
    }
}
