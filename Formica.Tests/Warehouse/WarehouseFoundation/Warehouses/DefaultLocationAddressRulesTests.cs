using System.Net.Http.Json;
using Formica.ApiService.Warehouse.WarehouseFoundation.Contracts.Warehouses;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;
using Formica.Tests.Warehouse.WarehouseFoundation.TestData;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Warehouses;

public sealed class DefaultLocationAddressRulesTests(WarehouseFoundationApiFixture fixture) : IClassFixture<WarehouseFoundationApiFixture>
{
    [Fact]
    public async Task CreatingWarehouseProvidesDefaultLocationAddressRules()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var createResponse = await fixture.ApiClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            new CreateWarehouseRequest(WarehouseFoundationTestData.UniqueWarehouseCode(), "Warehouse With Rules", null),
            cancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
        Assert.NotNull(created);

        var rulesResponse = await fixture.ApiClient.GetAsync(
            "/api/warehouse-foundation/location-address-rules/default",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, rulesResponse.StatusCode);

        var rules = await rulesResponse.Content.ReadFromJsonAsync<LocationAddressRulesResponse>(cancellationToken);
        Assert.NotNull(rules);
        Assert.Equal(LocationAddressRules.DefaultMaxLength, rules.MaxLength);
        Assert.True(rules.NormalizeToUppercase);
        Assert.True(rules.TrimWhitespace);
        Assert.False(rules.ZonePrefixRequired);
    }
}
