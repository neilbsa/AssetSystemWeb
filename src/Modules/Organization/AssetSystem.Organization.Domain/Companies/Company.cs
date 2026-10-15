using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.Companies;

public sealed class Company : AggregateRoot, IAuditable
{
    private Company() { }

    private Company(Guid id, string code, string name, string address, string assetTag) : base(id)
    {
        Code = code;
        Name = name;
        Address = address;
        AssetTag = assetTag;
    }

    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;

    /// <summary>Short prefix used when generating asset item ids, e.g. "CVC".</summary>
    public string AssetTag { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Company Create(string code, string name, string address, string assetTag) =>
        new(NewId(), Guard.NotBlank(code, nameof(Code), 20), Guard.NotBlank(name, nameof(Name)),
            Guard.OptionalText(address, nameof(Address)) ?? "", Guard.NotBlank(assetTag, nameof(AssetTag), 10).ToUpperInvariant());

    public void Update(string name, string address, string assetTag)
    {
        Name = Guard.NotBlank(name, nameof(Name));
        Address = Guard.OptionalText(address, nameof(Address)) ?? "";
        AssetTag = Guard.NotBlank(assetTag, nameof(AssetTag), 10).ToUpperInvariant();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
