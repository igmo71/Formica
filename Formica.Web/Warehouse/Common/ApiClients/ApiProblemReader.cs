using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Formica.Web.Warehouse.Common.ApiClients;

public static class ApiProblemReader
{
    public static async Task<string> ReadErrorMessageAsync(
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

        var validationErrors = ReadValidationErrors(problem);
        if (validationErrors.Count > 0)
        {
            return string.Join(" ", validationErrors);
        }

        return !string.IsNullOrWhiteSpace(problem.Detail)
            ? problem.Detail
            : problem.Title ?? fallback;
    }

    private static List<string> ReadValidationErrors(ProblemDetails problem)
    {
        if (!problem.Extensions.TryGetValue("errors", out var errorsObject) ||
            errorsObject is not JsonElement errorsElement ||
            errorsElement.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        var errors = new List<string>();
        foreach (var property in errorsElement.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Array)
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
