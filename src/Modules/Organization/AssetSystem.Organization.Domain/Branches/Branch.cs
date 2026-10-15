using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.Branches;

public sealed class Branch : AggregateRoot, IAuditable
{
    private Branch() { }

    private Branch(Guid id, Guid companyId, string name, string address, string assetTag) : base(id)
    {
        CompanyId = companyId;
        Name = name;
        Address = address;
        AssetTag = assetTag;
    }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;

    /// <summary>Optional prefix for item ids created at this branch, e.g. "DVO". Empty for head office.</summary>
    public string AssetTag { get; private set; } = default!;

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Branch Create(Guid companyId, string name, string address, string? assetTag) =>
        new(NewId(), companyId, Guard.NotBlank(name, nameof(Name)),
            Guard.OptionalText(address, nameof(Address)) ?? "",
            (Guard.OptionalText(assetTag, nameof(AssetTag), 10) ?? "").ToUpperInvariant());

    public void Update(string name, string address, string? assetTag)
    {
        Name = Guard.NotBlank(name, nameof(Name));
        Address = Guard.OptionalText(address, nameof(Address)) ?? "";
        AssetTag = (Guard.OptionalText(assetTag, nameof(AssetTag), 10) ?? "").ToUpperInvariant();
    }

    public void Deactivate() => IsActive = false;
}
