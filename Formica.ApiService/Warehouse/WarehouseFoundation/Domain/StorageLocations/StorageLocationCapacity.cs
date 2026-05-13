using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.StorageLocations;

public sealed record StorageLocationCapacity
{
    private StorageLocationCapacity(
        decimal? maxWeight,
        decimal? volume,
        decimal? height,
        decimal? width,
        decimal? depth)
    {
        MaxWeight = maxWeight;
        Volume = volume;
        Height = height;
        Width = width;
        Depth = depth;
    }

    public decimal? MaxWeight { get; }

    public decimal? Volume { get; }

    public decimal? Height { get; }

    public decimal? Width { get; }

    public decimal? Depth { get; }

    public static DomainValidationResult Validate(
        decimal? maxWeight = null,
        decimal? volume = null,
        decimal? height = null,
        decimal? width = null,
        decimal? depth = null)
    {
        var errors = new List<DomainValidationFailure>();

        AddNonNegativeError(errors, maxWeight, nameof(MaxWeight));
        AddNonNegativeError(errors, volume, nameof(Volume));
        AddNonNegativeError(errors, height, nameof(Height));
        AddNonNegativeError(errors, width, nameof(Width));
        AddNonNegativeError(errors, depth, nameof(Depth));

        return DomainValidationResult.Invalid(errors);
    }

    public static DomainValidationResult TryCreate(
        out StorageLocationCapacity? capacity,
        decimal? maxWeight = null,
        decimal? volume = null,
        decimal? height = null,
        decimal? width = null,
        decimal? depth = null)
    {
        var result = Validate(maxWeight, volume, height, width, depth);
        if (!result.IsValid)
        {
            capacity = null;
            return result;
        }

        capacity = new StorageLocationCapacity(maxWeight, volume, height, width, depth);
        return DomainValidationResult.Valid;
    }

    private static void AddNonNegativeError(List<DomainValidationFailure> errors, decimal? value, string field)
    {
        if (value < 0)
        {
            errors.Add(new(
                "StorageLocationCapacity.NegativeValue",
                $"{field} must be greater than or equal to zero.",
                field));
        }
    }
}
