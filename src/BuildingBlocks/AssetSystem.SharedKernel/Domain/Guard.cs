namespace AssetSystem.SharedKernel.Domain;

/// <summary>Small helpers that keep invariant checks in aggregates one line long.</summary>
public static class Guard
{
    public static string NotBlank(string? value, string field, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field}.required", $"{field} is required.");
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"{field}.too_long", $"{field} must be at most {maxLength} characters.");
        return trimmed;
    }

    public static string? OptionalText(string? value, string field, int maxLength = 500)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"{field}.too_long", $"{field} must be at most {maxLength} characters.");
        return trimmed;
    }

    public static void That(bool condition, string code, string message)
    {
        if (!condition) throw new DomainException(code, message);
    }
}
