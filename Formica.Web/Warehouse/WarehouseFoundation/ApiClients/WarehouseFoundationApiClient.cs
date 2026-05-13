using System.Net;
using System.Net.Http.Json;

namespace Formica.Web.Warehouse.WarehouseFoundation.ApiClients;

public sealed class WarehouseFoundationApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WarehouseResponse>> ListWarehousesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<WarehouseResponse[]>(
            $"/api/warehouse-foundation/warehouses?includeInactive={includeInactive.ToString().ToLowerInvariant()}",
            cancellationToken) ?? [];

    public async Task<WarehouseResponse?> CreateWarehouseAsync(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict ||
            response.StatusCode == HttpStatusCode.BadRequest)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
    }

    public async Task<WarehouseResponse?> UpdateWarehouseAsync(
        Guid warehouseId,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}",
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
    }

    public async Task<WarehouseResponse?> DeactivateWarehouseAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default)
        => await SendLifecycleCommandAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}/deactivate",
            cancellationToken);

    public async Task<WarehouseResponse?> ReactivateWarehouseAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default)
        => await SendLifecycleCommandAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}/reactivate",
            cancellationToken);

    private async Task<WarehouseResponse?> SendLifecycleCommandAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(path, content: null, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Conflict)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken);
    }
}

public sealed record WarehouseResponse(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record CreateWarehouseRequest(
    string? Code,
    string? Name,
    string? Description);

public sealed record UpdateWarehouseRequest(
    string? Code,
    string? Name,
    string? Description);
