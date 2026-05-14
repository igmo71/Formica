namespace Formica.Web.Warehouse.Common.ApiClients;

public sealed record ApiClientResult<T>(T? Value, string? ErrorMessage)
{
    public bool IsSuccess => ErrorMessage is null;

    public static ApiClientResult<T> Success(T? value) => new(value, null);

    public static ApiClientResult<T> Failure(string errorMessage) => new(default, errorMessage);
}
