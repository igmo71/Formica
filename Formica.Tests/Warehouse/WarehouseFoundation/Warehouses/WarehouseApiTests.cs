using System.Net.Http.Json;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.Tests.Warehouse.WarehouseFoundation.TestData;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Warehouses;

public sealed class WarehouseApiTests(WarehouseFoundationApiFixture fixture) : IClassFixture<WarehouseFoundationApiFixture>
{
    [Fact]
    public async Task WarehouseLifecyclePreservesIdentityAndFiltersInactiveByDefault()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var code = WarehouseFoundationTestData.UniqueWarehouseCode();

        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            new CreateWarehouseRequest(code, "Main Warehouse", "Primary warehouse"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(created);
        Assert.Equal(code, created.Code);
        Assert.Equal("Main Warehouse", created.Name);
        Assert.True(created.IsActive);

        var getResponse = await fixture.ApiClient.GetAsync(
            $"/api/warehouse-foundation/warehouses/{created.Id}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);

        var updateResponse = await fixture.ApiClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/warehouses/{created.Id}",
            new UpdateWarehouseRequest($"{code}-U"[..Math.Min(code.Length + 2, 32)], "Updated Warehouse", "Updated description"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Updated Warehouse", updated.Name);
        Assert.True(updated.IsActive);

        var deactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/warehouses/{created.Id}/deactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);
        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(deactivated);
        Assert.Equal(created.Id, deactivated.Id);
        Assert.False(deactivated.IsActive);

        var activeWarehouses = await fixture.ApiClient.GetFromJsonAsync<WarehouseResponse[]>(
            "/api/warehouse-foundation/warehouses",
            cancellationToken);

        Assert.DoesNotContain(activeWarehouses ?? [], warehouse => warehouse.Id == created.Id);

        var allWarehouses = await fixture.ApiClient.GetFromJsonAsync<WarehouseResponse[]>(
            "/api/warehouse-foundation/warehouses?includeInactive=true",
            cancellationToken);

        Assert.Contains(allWarehouses ?? [], warehouse => warehouse.Id == created.Id && !warehouse.IsActive);

        var reactivateResponse = await fixture.ApiClient.PostAsync(
            $"/api/warehouse-foundation/warehouses/{created.Id}/reactivate",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, reactivateResponse.StatusCode);
        var reactivated = await reactivateResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(reactivated);
        Assert.Equal(created.Id, reactivated.Id);
        Assert.True(reactivated.IsActive);
    }

    [Fact]
    public async Task DuplicateWarehouseCodeIsRejected()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var code = WarehouseFoundationTestData.UniqueWarehouseCode();

        var firstResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            new CreateWarehouseRequest(code, "First Warehouse", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            new CreateWarehouseRequest(code.ToLowerInvariant(), "Duplicate Warehouse", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }
}
