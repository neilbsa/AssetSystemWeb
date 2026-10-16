namespace AssetSystem.SharedKernel.Domain;

/// <summary>Filled in automatically by the persistence layer; never set from domain code.</summary>
public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
