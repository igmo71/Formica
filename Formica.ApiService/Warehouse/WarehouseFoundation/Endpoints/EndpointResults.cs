using Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Endpoints;

public static class EndpointResults
{
    private const string ConflictProblemType = "https://formica/problems/conflict";

    public static ValidationProblem ValidationProblem(IEnumerable<FeatureError> validationErrors)
    {
        var errors = validationErrors
            .GroupBy(error => error.Field ?? error.Code)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());

        return TypedResults.ValidationProblem(errors);
    }

    public static Conflict<ProblemDetails> Conflict(string detail, string? code = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = ConflictProblemType,
            Title = "Conflict",
            Status = StatusCodes.Status409Conflict,
            Detail = detail
        };

        if (!string.IsNullOrWhiteSpace(code))
        {
            problemDetails.Extensions["code"] = code;
        }

        return TypedResults.Conflict(problemDetails);
    }

    public static NotFound NotFound() => TypedResults.NotFound();
}
