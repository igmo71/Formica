using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Features.Common;

public sealed record FeatureResult<T>(
    FeatureResultStatus Status,
    T? Value = default,
    IReadOnlyList<FeatureError>? Errors = null)
{
    public bool IsSuccess => Status == FeatureResultStatus.Success;

    public IReadOnlyList<FeatureError> ErrorList => Errors ?? [];

    public FeatureError? FirstError => ErrorList.FirstOrDefault();

    public static FeatureResult<T> Success(T value)
        => new(FeatureResultStatus.Success, value);

    public static FeatureResult<T> ValidationFailed(IEnumerable<DomainValidationFailure> errors)
        => new(
            FeatureResultStatus.ValidationFailed,
            Errors: errors.Select(error => new FeatureError(error.Code, error.Message, error.Field)).ToArray());

    public static FeatureResult<T> Conflict(string code, string message, string? field = null)
        => new(
            FeatureResultStatus.Conflict,
            Errors: [new FeatureError(code, message, field)]);

    public static FeatureResult<T> NotFound(string code, string message, string? field = null)
        => new(
            FeatureResultStatus.NotFound,
            Errors: [new FeatureError(code, message, field)]);
}
