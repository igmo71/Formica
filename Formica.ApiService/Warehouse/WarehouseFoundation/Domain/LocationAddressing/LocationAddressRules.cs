using System.Text.RegularExpressions;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common;
using Formica.ApiService.Warehouse.WarehouseFoundation.Domain.Common.Validation;

namespace Formica.ApiService.Warehouse.WarehouseFoundation.Domain.LocationAddressing;

public sealed class LocationAddressRules : EntityLifecycle
{
    public const string DefaultCode = "DEFAULT";
    public const int DefaultMaxLength = 50;
    public const string DefaultAllowedPattern = "^[A-Z0-9][A-Z0-9\\-_.]*$";

    private LocationAddressRules(
        string code,
        int maxLength,
        string? allowedPattern,
        bool normalizeToUppercase,
        bool trimWhitespace,
        bool zonePrefixRequired)
    {
        Code = code;
        MaxLength = maxLength;
        AllowedPattern = allowedPattern;
        NormalizeToUppercase = normalizeToUppercase;
        TrimWhitespace = trimWhitespace;
        ZonePrefixRequired = zonePrefixRequired;
    }

    private LocationAddressRules()
    {
        Code = DefaultCode;
        MaxLength = DefaultMaxLength;
        AllowedPattern = DefaultAllowedPattern;
        NormalizeToUppercase = true;
        TrimWhitespace = true;
        ZonePrefixRequired = false;
    }

    public string Code { get; private set; }

    public int MaxLength { get; private set; }

    public string? AllowedPattern { get; private set; }

    public bool NormalizeToUppercase { get; private set; }

    public bool TrimWhitespace { get; private set; }

    public bool ZonePrefixRequired { get; private set; }

    public static LocationAddressRules CreateDefault()
        => new(DefaultCode, DefaultMaxLength, DefaultAllowedPattern, true, true, false);

    public static DomainValidationResult ValidateSettings(
        string code,
        int maxLength,
        string? allowedPattern)
    {
        var errors = new List<DomainValidationFailure>();

        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add(new(
                "LocationAddressRules.CodeRequired",
                "Location address rules code is required.",
                nameof(Code)));
        }

        if (maxLength <= 0)
        {
            errors.Add(new(
                "LocationAddressRules.MaxLengthInvalid",
                "Location address maximum length must be greater than zero.",
                nameof(MaxLength)));
        }

        if (!string.IsNullOrWhiteSpace(allowedPattern))
        {
            try
            {
                _ = new Regex(allowedPattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250));
            }
            catch (ArgumentException)
            {
                errors.Add(new(
                    "LocationAddressRules.PatternInvalid",
                    "Location address allowed pattern must be a valid regular expression.",
                    nameof(AllowedPattern)));
            }
        }

        return DomainValidationResult.Invalid(errors);
    }

    public string NormalizeAddress(string address)
    {
        var normalized = TrimWhitespace ? address.Trim() : address;

        return NormalizeToUppercase ? normalized.ToUpperInvariant() : normalized;
    }

    public DomainValidationResult ValidateAddress(string address, string? zoneCode = null)
    {
        var errors = new List<DomainValidationFailure>();
        var normalizedAddress = NormalizeAddress(address);

        if (string.IsNullOrWhiteSpace(normalizedAddress))
        {
            errors.Add(new(
                "LocationAddress.AddressRequired",
                "Location address is required.",
                "address"));

            return DomainValidationResult.Invalid(errors);
        }

        if (normalizedAddress.Length > MaxLength)
        {
            errors.Add(new(
                "LocationAddress.MaxLengthExceeded",
                $"Location address must not exceed {MaxLength} characters.",
                "address"));
        }

        if (!string.IsNullOrWhiteSpace(AllowedPattern) &&
            !Regex.IsMatch(normalizedAddress, AllowedPattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250)))
        {
            errors.Add(new(
                "LocationAddress.PatternMismatch",
                "Location address contains characters that are not allowed.",
                "address"));
        }

        if (ZonePrefixRequired && !string.IsNullOrWhiteSpace(zoneCode))
        {
            var normalizedZoneCode = TrimWhitespace ? zoneCode.Trim() : zoneCode;
            normalizedZoneCode = NormalizeToUppercase ? normalizedZoneCode.ToUpperInvariant() : normalizedZoneCode;

            if (!normalizedAddress.StartsWith(normalizedZoneCode, StringComparison.Ordinal))
            {
                errors.Add(new(
                    "LocationAddress.ZonePrefixRequired",
                    "Location address must start with the zone code.",
                    "address"));
            }
        }

        return DomainValidationResult.Invalid(errors);
    }
}
