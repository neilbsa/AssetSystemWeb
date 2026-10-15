namespace AssetSystem.SharedKernel.Domain;

/// <summary>Thrown when an aggregate invariant would be violated. Surfaces as HTTP 422.</summary>
public sealed class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}
