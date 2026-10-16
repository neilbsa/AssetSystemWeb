using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Assets.Domain.Assets;

/// <summary>
/// A purchased bundle of equipment (e.g. one desktop computer with its monitor, keyboard and mouse),
/// tracked item by item. The aggregate guards the bundle's consistency: item identity, status rules and attachments.
/// </summary>
public sealed class Asset : AggregateRoot, IAuditable
{
    private readonly List<AssetItem> _items = [];
    private readonly List<AssetDocument> _documents = [];

    private Asset() { }

    public Guid CompanyId { get; private set; }
    public AssetCategory Category { get; private set; }

    /// <summary>Unique number such as "2CS0001".</summary>
    public string AssetNumber { get; private set; } = default!;

    public string Description { get; private set; } = default!;
    public string Vendor { get; private set; } = default!;
    public Guid BranchId { get; private set; }
    public string? PurchaseOrderNumber { get; private set; }
    public string? ControlNumber { get; private set; }
    public string? Remarks { get; private set; }

    public Purchase Purchase { get; private set; } = default!;
    public InvoiceInfo Invoice { get; private set; } = default!;

    public IReadOnlyCollection<AssetItem> Items => _items;
    public IReadOnlyCollection<AssetDocument> Documents => _documents;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Asset Register(
        Guid companyId, AssetCategory category, string assetNumber, Guid branchId, string description, string vendor,
        string? purchaseOrderNumber, string? controlNumber, string? remarks, Purchase purchase, InvoiceInfo invoice)
    {
        var asset = new Asset
        {
            Id = NewId(),
            CompanyId = companyId,
            Category = category,
            AssetNumber = Guard.NotBlank(assetNumber, nameof(AssetNumber), 30),
            BranchId = branchId,
            Purchase = purchase,
            Invoice = invoice
        };
        asset.SetDetails(description, vendor, purchaseOrderNumber, controlNumber, remarks);
        return asset;
    }

    public void UpdateDetails(
        string description, string vendor, string? purchaseOrderNumber, string? controlNumber, string? remarks,
        Purchase purchase, InvoiceInfo invoice)
    {
        SetDetails(description, vendor, purchaseOrderNumber, controlNumber, remarks);
        Purchase = purchase;
        Invoice = invoice;
    }

    public decimal DepreciatedValue(DateOnly today) => Purchase.DepreciatedValue(today);

    // ---- Items ----------------------------------------------------------------------------

    public AssetItem AddItem(
        string itemCode, string typeCode, string typeName, string description, string partNumber,
        string serialNumber, string keyDetail, string? controlNumber, string? remarks)
    {
        var item = new AssetItem(CompanyId, Guard.NotBlank(itemCode, nameof(AssetItem.ItemCode), 50),
            Guard.NotBlank(typeCode, nameof(AssetItem.TypeCode), 20), Guard.NotBlank(typeName, nameof(AssetItem.TypeName)),
            description, partNumber, serialNumber, keyDetail, controlNumber, remarks);
        _items.Add(item);
        return item;
    }

    public void UpdateItem(
        Guid itemId, string description, string partNumber, string serialNumber, string keyDetail,
        string? controlNumber, string? remarks) =>
        GetItem(itemId).Describe(description, partNumber, serialNumber, keyDetail, controlNumber, remarks);

    public void ChangeItemStatus(Guid itemId, ItemStatus status) => GetItem(itemId).ChangeStatus(status);

    /// <summary>Marks the given items as handed out. Throws if any item is not available.</summary>
    public void IssueItems(IEnumerable<Guid> itemIds)
    {
        foreach (var id in itemIds) GetItem(id).Issue();
    }

    public AssetItem? FindItem(Guid itemId) => _items.FirstOrDefault(i => i.Id == itemId);

    private AssetItem GetItem(Guid itemId) =>
        FindItem(itemId) ?? throw new DomainException("Item.not_found", "Item does not belong to this asset.");

    // ---- Documents ------------------------------------------------------------------------

    public AssetDocument AttachDocument(string documentType, string fileName, string contentType, long sizeBytes, string storageKey)
    {
        var document = new AssetDocument(documentType, fileName, contentType, sizeBytes, storageKey);
        _documents.Add(document);
        return document;
    }

    /// <summary>Detaches the document and returns it so the caller can delete the stored file.</summary>
    public AssetDocument RemoveDocument(Guid documentId)
    {
        var document = _documents.FirstOrDefault(d => d.Id == documentId)
                       ?? throw new DomainException("Document.not_found", "Document does not belong to this asset.");
        _documents.Remove(document);
        return document;
    }

    private void SetDetails(string description, string vendor, string? purchaseOrderNumber, string? controlNumber, string? remarks)
    {
        Description = Guard.NotBlank(description, nameof(Description));
        Vendor = Guard.NotBlank(vendor, nameof(Vendor));
        PurchaseOrderNumber = Guard.OptionalText(purchaseOrderNumber, nameof(PurchaseOrderNumber), 100);
        ControlNumber = Guard.OptionalText(controlNumber, nameof(ControlNumber), 100);
        Remarks = Guard.OptionalText(remarks, nameof(Remarks));
    }
}
