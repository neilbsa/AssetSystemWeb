using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Assets.Domain.Assets;

/// <summary>Metadata of a file attached to an asset (invoice scan, warranty card ...). The bytes live in the file store.</summary>
public sealed class AssetDocument : Entity
{
    private AssetDocument() { }

    internal AssetDocument(string documentType, string fileName, string contentType, long sizeBytes, string storageKey)
        : base(NewId())
    {
        DocumentType = Guard.NotBlank(documentType, nameof(DocumentType), 100);
        FileName = Guard.NotBlank(Path.GetFileName(fileName), nameof(FileName), 255);
        ContentType = Guard.NotBlank(contentType, nameof(ContentType), 150);
        SizeBytes = sizeBytes;
        StorageKey = storageKey;
    }

    public string DocumentType { get; private set; } = default!;
    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; } = default!;
}
