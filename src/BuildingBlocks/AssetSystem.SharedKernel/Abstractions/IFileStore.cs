namespace AssetSystem.SharedKernel.Abstractions;

/// <summary>Binary storage for uploaded documents (asset papers, receiving copies).</summary>
public interface IFileStore
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct);
    Task<Stream?> OpenAsync(string storageKey, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
}
