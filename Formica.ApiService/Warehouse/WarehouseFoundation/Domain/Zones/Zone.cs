using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Zones;

public sealed class Zone : EntityLifecycle
{
    public const int MaxCodeLength = 32;
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;

    private Zone(Guid warehouseId, string code, string name, ZonePurpose purpose, string? description)
    {
        WarehouseId = warehouseId;
        Code = code;
        Name = name;
        Purpose = purpose;
        Description = NormalizeOptionalText(description);
    }

    private Zone()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public Guid WarehouseId { get; private set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public ZonePurpose Purpose { get; private set; }

    public string? Description { get; private set; }

    public static DomainValidationResult TryCreate(
        Guid warehouseId,
        string? code,
        string? name,
        ZonePurpose purpose,
        string? description,
        out Zone? zone)
    {
        var validationResult = Validate(warehouseId, code, name, purpose, description);
        if (!validationResult.IsValid)
        {
            zone = null;
            return validationResult;
        }

        zone = new Zone(
            warehouseId,
            NormalizeCode(code),
            NormalizeRequiredText(name),
            purpose,
            description);

        return DomainValidationResult.Valid;
    }

    public void Update(string? code, string? name, ZonePurpose purpose, string? description)
    {
        Code = NormalizeCode(code);
        Name = NormalizeRequiredText(name);
        Purpose = purpose;
        Description = NormalizeOptionalText(description);
        Touch();
    }

    public static DomainValidationResult Validate(
        Guid warehouseId,
        string? code,
        string? name,
        ZonePurpose purpose,
        string? description)
    {
        var errors = new List<DomainValidationFailure>();

        if (warehouseId == Guid.Empty)
        {
            errors.Add(new(
                "Zone.WarehouseRequired",
                "Zone warehouse is required.",
                "warehouseId"));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add(new(
                "Zone.CodeRequired",
                "Zone code is required.",
                "code"));
        }
        else if (NormalizeCode(code).Length > MaxCodeLength)
        {
            errors.Add(new(
                "Zone.CodeTooLong",
                $"Zone code must not exceed {MaxCodeLength} characters.",
                "code"));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new(
                "Zone.NameRequired",
                "Zone name is required.",
                "name"));
        }
        else if (NormalizeRequiredText(name).Length > MaxNameLength)
        {
            errors.Add(new(
                "Zone.NameTooLong",
                $"Zone name must not exceed {MaxNameLength} characters.",
                "name"));
        }

        if (!Enum.IsDefined(purpose))
        {
            errors.Add(new(
                "Zone.PurposeRequired",
                "Zone purpose is required.",
                "purpose"));
        }

        if (NormalizeOptionalText(description)?.Length > MaxDescriptionLength)
        {
            errors.Add(new(
                "Zone.DescriptionTooLong",
                $"Zone description must not exceed {MaxDescriptionLength} characters.",
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
