using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Assets.Application.Features.Documents;

public static class DocumentRules
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    public static readonly IReadOnlySet<string> AllowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "image/png", "image/jpeg", "image/webp",
        "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };
}

// ---- Upload -------------------------------------------------------------------------------

/// <summary>Content is a stream owned by the caller (the HTTP request); it is read only while the handler runs.</summary>
public sealed record UploadAssetDocumentCommand(
    Guid CompanyId, Guid AssetId, string DocumentType, string FileName, string ContentType, long SizeBytes, Stream Content);

public sealed class UploadAssetDocumentValidator : AbstractValidator<UploadAssetDocumentCommand>
{
    public UploadAssetDocumentValidator()
    {
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SizeBytes).GreaterThan(0).LessThanOrEqualTo(DocumentRules.MaxSizeBytes)
            .WithMessage("File must be between 1 byte and 10 MB.");
        RuleFor(x => x.ContentType).Must(DocumentRules.AllowedContentTypes.Contains)
            .WithMessage("Only PDF, image, Word and Excel files are accepted.");
    }
}

public static class UploadAssetDocumentHandler
{
    public static async Task<Result<Guid>> Handle(
        UploadAssetDocumentCommand command, IAssetRepository assets, IFileStore files, IAssetsUnitOfWork uow, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");

        var key = await files.SaveAsync(command.Content, command.FileName, ct);
        try
        {
            var document = asset.AttachDocument(command.DocumentType, command.FileName, command.ContentType, command.SizeBytes, key);
            await uow.CommitAsync(ct);
            return document.Id;
        }
        catch
        {
            await files.DeleteAsync(key, CancellationToken.None); // do not leave orphaned files behind
            throw;
        }
    }
}

// ---- Download -----------------------------------------------------------------------------

public sealed record DownloadAssetDocumentQuery(Guid CompanyId, Guid AssetId, Guid DocumentId);

public sealed record DocumentDownload(Stream Content, string FileName, string ContentType);

public static class DownloadAssetDocumentHandler
{
    public static async Task<Result<DocumentDownload>> Handle(
        DownloadAssetDocumentQuery query, IAssetRepository assets, IFileStore files, CancellationToken ct)
    {
        var asset = await assets.GetAsync(query.CompanyId, query.AssetId, ct);
        var document = asset?.Documents.FirstOrDefault(d => d.Id == query.DocumentId);
        if (document is null) return Error.NotFound("document.not_found", "Document not found.");

        var stream = await files.OpenAsync(document.StorageKey, ct);
        return stream is null
            ? Error.NotFound("document.file_missing", "The stored file is missing.")
            : new DocumentDownload(stream, document.FileName, document.ContentType);
    }
}

// ---- Remove -------------------------------------------------------------------------------

public sealed record RemoveAssetDocumentCommand(Guid CompanyId, Guid AssetId, Guid DocumentId);

public static class RemoveAssetDocumentHandler
{
    public static async Task<Result> Handle(
        RemoveAssetDocumentCommand command, IAssetRepository assets, IFileStore files, IAssetsUnitOfWork uow, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");

        var document = asset.RemoveDocument(command.DocumentId);
        await uow.CommitAsync(ct);
        await files.DeleteAsync(document.StorageKey, ct); // only after the metadata is gone
        return Result.Success();
    }
}
