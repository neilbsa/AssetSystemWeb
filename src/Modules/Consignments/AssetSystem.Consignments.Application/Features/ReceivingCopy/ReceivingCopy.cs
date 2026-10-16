using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Consignments.Application.Features.ReceivingCopy;

public static class ReceivingCopyRules
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;

    public static readonly IReadOnlySet<string> AllowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "image/png", "image/jpeg", "image/webp"
    };
}

public sealed record UploadReceivingCopyCommand(
    Guid CompanyId, Guid ConsignmentId, string FileName, string ContentType, long SizeBytes, Stream Content);

public sealed class UploadReceivingCopyValidator : AbstractValidator<UploadReceivingCopyCommand>
{
    public UploadReceivingCopyValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SizeBytes).GreaterThan(0).LessThanOrEqualTo(ReceivingCopyRules.MaxSizeBytes)
            .WithMessage("File must be between 1 byte and 10 MB.");
        RuleFor(x => x.ContentType).Must(ReceivingCopyRules.AllowedContentTypes.Contains)
            .WithMessage("Only PDF and image files are accepted.");
    }
}

public static class UploadReceivingCopyHandler
{
    public static async Task<Result> Handle(
        UploadReceivingCopyCommand command, IConsignmentRepository consignments, IFileStore files,
        IConsignmentsUnitOfWork uow, CancellationToken ct)
    {
        var consignment = await consignments.GetAsync(command.CompanyId, command.ConsignmentId, ct);
        if (consignment is null) return Error.NotFound("consignment.not_found", "Consignment not found.");

        var key = await files.SaveAsync(command.Content, command.FileName, ct);
        try
        {
            var replaced = consignment.AttachReceivingCopy(command.FileName, command.ContentType, command.SizeBytes, key);
            await uow.CommitAsync(ct);
            if (replaced is not null) await files.DeleteAsync(replaced.StorageKey, ct);
            return Result.Success();
        }
        catch
        {
            await files.DeleteAsync(key, CancellationToken.None);
            throw;
        }
    }
}

public sealed record DownloadReceivingCopyQuery(Guid CompanyId, Guid ConsignmentId);

public sealed record ReceivingCopyDownload(Stream Content, string FileName, string ContentType);

public static class DownloadReceivingCopyHandler
{
    public static async Task<Result<ReceivingCopyDownload>> Handle(
        DownloadReceivingCopyQuery query, IConsignmentRepository consignments, IFileStore files, CancellationToken ct)
    {
        var consignment = await consignments.GetAsync(query.CompanyId, query.ConsignmentId, ct);
        var copy = consignment?.ReceivingCopy;
        if (copy is null) return Error.NotFound("receiving_copy.not_found", "No receiving copy has been attached.");

        var stream = await files.OpenAsync(copy.StorageKey, ct);
        return stream is null
            ? Error.NotFound("receiving_copy.file_missing", "The stored file is missing.")
            : new ReceivingCopyDownload(stream, copy.FileName, copy.ContentType);
    }
}
