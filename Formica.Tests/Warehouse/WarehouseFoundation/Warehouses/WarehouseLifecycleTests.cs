using Formica.Tests.Warehouse.WarehouseFoundation.TestData;

namespace Formica.Tests.Warehouse.WarehouseFoundation.Warehouses;

public sealed class WarehouseLifecycleTests
{
    [Fact]
    public void TryUpdateWithValidAttributesMutatesNormalizedFields()
    {
        var warehouse = WarehouseFoundationTestData.Warehouse(code: "WH-TEST", name: "Test Warehouse");
        var id = warehouse.Id;
        var createdAtUtc = warehouse.CreatedAtUtc;

        var validationResult = warehouse.TryUpdate(" wh-updated ", " Updated Warehouse ", " Updated description ");

        Assert.True(validationResult.IsValid);
        Assert.Equal(id, warehouse.Id);
        Assert.Equal(createdAtUtc, warehouse.CreatedAtUtc);
        Assert.Equal("WH-UPDATED", warehouse.Code);
        Assert.Equal("Updated Warehouse", warehouse.Name);
        Assert.Equal("Updated description", warehouse.Description);
        Assert.True(warehouse.UpdatedAtUtc >= createdAtUtc);
    }

    [Theory]
    [InlineData(" ", "Test Warehouse", "Warehouse.CodeRequired")]
    [InlineData("WH-TEST", " ", "Warehouse.NameRequired")]
    public void TryUpdateWithInvalidCodeOrNameReturnsValidationFailureAndDoesNotMutate(
        string code,
        string name,
        string expectedErrorCode)
    {
        var warehouse = WarehouseFoundationTestData.Warehouse(code: "WH-TEST", name: "Test Warehouse");
        var id = warehouse.Id;
        var originalCode = warehouse.Code;
        var originalName = warehouse.Name;
        var originalDescription = warehouse.Description;
        var originalUpdatedAtUtc = warehouse.UpdatedAtUtc;

        var validationResult = warehouse.TryUpdate(code, name, "Changed description");

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, error => error.Code == expectedErrorCode);
        Assert.Equal(id, warehouse.Id);
        Assert.Equal(originalCode, warehouse.Code);
        Assert.Equal(originalName, warehouse.Name);
        Assert.Equal(originalDescription, warehouse.Description);
        Assert.Equal(originalUpdatedAtUtc, warehouse.UpdatedAtUtc);
    }
}
