namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class WarehouseFoundationEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseFoundationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/warehouse-foundation")
            .WithTags("Warehouse Foundation");

        group.MapGet("/", () => Results.Ok(new WarehouseFoundationStatusResponse("Warehouse Foundation endpoints are registered.")))
            .WithName("GetWarehouseFoundationStatus");

        group.MapWarehouseEndpoints();

        return endpoints;
    }

    private sealed record WarehouseFoundationStatusResponse(string Message);
}
