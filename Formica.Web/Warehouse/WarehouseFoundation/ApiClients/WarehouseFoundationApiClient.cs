using System.Net.Http.Json;
using Formica.Web.Warehouse.Common.ApiClients;

namespace Formica.Web.Warehouse.WarehouseFoundation.ApiClients;

public sealed class WarehouseFoundationApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WarehouseResponse>> ListWarehousesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<WarehouseResponse[]>(
            $"/api/warehouse-foundation/warehouses?includeInactive={includeInactive.ToString().ToLowerInvariant()}",
            cancellationToken) ?? [];

    public async Task<ApiClientResult<WarehouseResponse>> CreateWarehouseAsync(
        CreateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/warehouse-foundation/warehouses",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return ApiClientResult<WarehouseResponse>.Failure(
                await ApiProblemReader.ReadErrorMessageAsync(response, cancellationToken));
        }

        return ApiClientResult<WarehouseResponse>.Success(
            await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken));
    }

    public async Task<ApiClientResult<WarehouseResponse>> UpdateWarehouseAsync(
        Guid warehouseId,
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return ApiClientResult<WarehouseResponse>.Failure(
                await ApiProblemReader.ReadErrorMessageAsync(response, cancellationToken));
        }

        return ApiClientResult<WarehouseResponse>.Success(
            await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken));
    }

    public async Task<ApiClientResult<WarehouseResponse>> DeactivateWarehouseAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default)
        => await SendLifecycleCommandAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}/deactivate",
            cancellationToken);

    public async Task<ApiClientResult<WarehouseResponse>> ReactivateWarehouseAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default)
        => await SendLifecycleCommandAsync(
            $"/api/warehouse-foundation/warehouses/{warehouseId}/reactivate",
            cancellationToken);

    private async Task<ApiClientResult<WarehouseResponse>> SendLifecycleCommandAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(path, content: null, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return ApiClientResult<WarehouseResponse>.Failure(
                await ApiProblemReader.ReadErrorMessageAsync(response, cancellationToken));
        }

        return ApiClientResult<WarehouseResponse>.Success(
            await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken));
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
