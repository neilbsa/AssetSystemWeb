using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Consignments.Domain.Consignments;

/// <summary>Signed receiving copy scanned and attached to a consignment.</summary>
public sealed class ReceivingCopy : ValueObject
{
    private ReceivingCopy() { }

    public ReceivingCopy(string fileName, string contentType, long sizeBytes, string storageKey)
    {
        FileName = Guard.NotBlank(Path.GetFileName(fileName), nameof(FileName), 255);
        ContentType = Guard.NotBlank(contentType, nameof(ContentType), 150);
        SizeBytes = sizeBytes;
        StorageKey = Guard.NotBlank(storageKey, nameof(StorageKey), 64);
    }

    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; } = default!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StorageKey;
    }
}
