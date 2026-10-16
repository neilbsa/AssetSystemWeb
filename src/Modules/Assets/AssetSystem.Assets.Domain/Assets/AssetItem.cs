using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Assets.Domain.Assets;

/// <summary>A single trackable component of an asset (one monitor, one keyboard, ...). Only reachable through its <see cref="Asset"/>.</summary>
public sealed class AssetItem : Entity
{
    public const string NotApplicable = "N/A";

    private AssetItem() { }

    internal AssetItem(
        Guid companyId, string itemCode, string typeCode, string typeName, string description, string partNumber,
        string serialNumber, string keyDetail, string? controlNumber, string? remarks) : base(NewId())
    {
        CompanyId = companyId;
        ItemCode = itemCode;
        TypeCode = typeCode;
        TypeName = typeName;
        Status = ItemStatus.Available;
        Describe(description, partNumber, serialNumber, keyDetail, controlNumber, remarks);
    }

    public Guid CompanyId { get; private set; }

    /// <summary>Human friendly unique code printed on asset tags, e.g. "CVCDVOM-001".</summary>
    public string ItemCode { get; private set; } = default!;

    public string TypeCode { get; private set; } = default!;
    public string TypeName { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string PartNumber { get; private set; } = default!;

    /// <summary>Upper-cased; "N/A" when the item has none. Real serial numbers are unique per company.</summary>
    public string SerialNumber { get; private set; } = default!;

    /// <summary>License key / secondary identifier; upper-cased; "N/A" when none.</summary>
    public string KeyDetail { get; private set; } = default!;

    public string? ControlNumber { get; private set; }
    public string? Remarks { get; private set; }
    public ItemStatus Status { get; private set; }

    public static string NormalizeIdentifier(string? value)
    {
        var trimmed = string.IsNullOrWhiteSpace(value) ? NotApplicable : value.Trim().ToUpperInvariant();
        return trimmed;
    }

    internal void Describe(
        string description, string partNumber, string serialNumber, string keyDetail, string? controlNumber, string? remarks)
    {
        Description = Guard.NotBlank(description, nameof(Description));
        PartNumber = Guard.NotBlank(partNumber, nameof(PartNumber), 100);
        SerialNumber = NormalizeIdentifier(serialNumber);
        KeyDetail = NormalizeIdentifier(keyDetail);
        ControlNumber = Guard.OptionalText(controlNumber, nameof(ControlNumber), 100);
        Remarks = Guard.OptionalText(remarks, nameof(Remarks));
    }

    /// <summary>Hands the item to someone. Only available stock can be issued.</summary>
    internal void Issue()
    {
        Guard.That(Status == ItemStatus.Available, "Item.not_available",
            $"Item {ItemCode} is {Status} and cannot be issued.");
        Status = ItemStatus.InUse;
    }

    /// <summary>Manual status changes. "In use" is only reached by issuing the item through a consignment.</summary>
    internal void ChangeStatus(ItemStatus newStatus)
    {
        Guard.That(newStatus != ItemStatus.InUse, "Item.use_consignment",
            "Items are marked in use by issuing them through a consignment.");
        Guard.That(newStatus != Status, "Item.same_status", $"Item {ItemCode} is already {Status}.");
        Status = newStatus;
    }
}
