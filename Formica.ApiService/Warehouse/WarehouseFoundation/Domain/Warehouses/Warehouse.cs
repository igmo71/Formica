using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Warehouses;

public sealed class Warehouse : EntityLifecycle
{
    public const int MaxCodeLength = 32;
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;

    private Warehouse(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = NormalizeOptionalText(description);
    }

    private Warehouse()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public static DomainValidationResult TryCreate(
        string? code,
        string? name,
        string? description,
        out Warehouse? warehouse)
    {
        var validationResult = Validate(code, name, description);
        if (!validationResult.IsValid)
        {
            warehouse = null;
            return validationResult;
        }

        warehouse = new Warehouse(NormalizeCode(code), NormalizeRequiredText(name), description);
        return DomainValidationResult.Valid;
    }

    public void Update(string? code, string? name, string? description)
    {
        Code = NormalizeCode(code);
        Name = NormalizeRequiredText(name);
        Description = NormalizeOptionalText(description);
        Touch();
    }

    public static DomainValidationResult Validate(string? code, string? name, string? description)
    {
        var errors = new List<DomainValidationFailure>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add(new(
                "Warehouse.CodeRequired",
                "Warehouse code is required.",
                "code"));
        }
        else if (NormalizeCode(code).Length > MaxCodeLength)
        {
            errors.Add(new(
                "Warehouse.CodeTooLong",
                $"Warehouse code must not exceed {MaxCodeLength} characters.",
                "code"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new(
                "Warehouse.NameRequired",
                "Warehouse name is required.",
                "name"));
        }
        else if (NormalizeRequiredText(name).Length > MaxNameLength)
        {
            errors.Add(new(
                "Warehouse.NameTooLong",
                $"Warehouse name must not exceed {MaxNameLength} characters.",
                "name"));
        }

        if (NormalizeOptionalText(description)?.Length > MaxDescriptionLength)
        {
            errors.Add(new(
                "Warehouse.DescriptionTooLong",
                $"Warehouse description must not exceed {MaxDescriptionLength} characters.",
                "description"));
        }

        return DomainValidationResult.Invalid(errors);
    }

    public static string NormalizeCode(string? code)
        => NormalizeRequiredText(code).ToUpperInvariant();

    private static string NormalizeRequiredText(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var normalized = value?.Trim();

        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
