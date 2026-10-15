using AssetSystem.SharedKernel.Abstractions;

namespace AssetSystem.BuildingBlocks.Infrastructure.Files;

public sealed class FileStoreOptions
{
    public const string Section = "FileStore";
    public string RootPath { get; set; } = "data/files";
}

/// <summary>Stores uploads on disk (a Docker volume in production). Keys are random, never user supplied.</summary>
public sealed class LocalFileStore(Microsoft.Extensions.Options.IOptions<FileStoreOptions> options) : IFileStore
{
    private readonly string _root = Path.GetFullPath(options.Value.RootPath);

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken ct)
    {
        Directory.CreateDirectory(_root);
        var key = Guid.CreateVersion7().ToString("N");
        await using var target = File.Create(PathFor(key));
        await content.CopyToAsync(target, ct);
        return key;
    }

    public Task<Stream?> OpenAsync(string storageKey, CancellationToken ct)
    {
        var path = PathFor(storageKey);
        Stream? stream = File.Exists(path) ? File.OpenRead(path) : null;
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var path = PathFor(storageKey);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private string PathFor(string key)
    {
        // Keys are generated here, but guard anyway against path traversal if one is ever persisted from outside.
        if (key.Any(c => !char.IsAsciiLetterOrDigit(c)))
            throw new ArgumentException("Invalid storage key.", nameof(key));
        return Path.Combine(_root, key);
    }
}
