using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.ItemTypes;

/// <summary>Kind of component an asset is made of (Monitor, Mouse, ...). The code is embedded in generated item ids.</summary>
public sealed class ItemType : AggregateRoot, IAuditable
{
    private ItemType() { }

    private ItemType(Guid id, string code, string name) : base(id)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static ItemType Create(string code, string name) =>
        new(NewId(), Guard.NotBlank(code, nameof(Code), 20).ToUpperInvariant(), Guard.NotBlank(name, nameof(Name)));
}
