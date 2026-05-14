using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

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
            return ApiClientResult<WarehouseResponse>.Failure(await ReadErrorMessageAsync(response, cancellationToken));
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
            return ApiClientResult<WarehouseResponse>.Failure(await ReadErrorMessageAsync(response, cancellationToken));
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
            return ApiClientResult<WarehouseResponse>.Failure(await ReadErrorMessageAsync(response, cancellationToken));
        }

        return ApiClientResult<WarehouseResponse>.Success(
            await response.Content.ReadFromJsonAsync<WarehouseResponse>(cancellationToken));
    }

    private static async Task<string> ReadErrorMessageAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var fallback = response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "The warehouse request is invalid.",
            HttpStatusCode.Conflict => "The warehouse conflicts with existing data.",
            HttpStatusCode.NotFound => "The warehouse was not found.",
            _ => "The warehouse request failed."
        };

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
        if (problem is null)
        {
            return fallback;
        }

        var errors = ReadValidationErrors(problem);
        if (errors.Count > 0)
        {
            return string.Join(" ", errors);
        }

        return !string.IsNullOrWhiteSpace(problem.Detail)
            ? problem.Detail
            : problem.Title ?? fallback;
    }

    private static List<string> ReadValidationErrors(ProblemDetails problem)
    {
        if (!problem.Extensions.TryGetValue("errors", out var errorsObject) ||
            errorsObject is not System.Text.Json.JsonElement errorsElement ||
            errorsElement.ValueKind != System.Text.Json.JsonValueKind.Object)
        {
            return [];
        }

        var errors = new List<string>();
        foreach (var property in errorsElement.EnumerateObject())
        {
            if (property.Value.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                continue;
            }

            foreach (var item in property.Value.EnumerateArray())
            {
                if (item.GetString() is { Length: > 0 } message)
                {
                    errors.Add(message);
                }
            }
        }

        return errors;
    }
}

public sealed record ApiClientResult<T>(T? Value, string? ErrorMessage)
{
    public bool IsSuccess => ErrorMessage is null;

    public static ApiClientResult<T> Success(T? value) => new(value, null);

    public static ApiClientResult<T> Failure(string errorMessage) => new(default, errorMessage);
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
