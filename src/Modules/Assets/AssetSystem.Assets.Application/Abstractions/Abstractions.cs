using AssetSystem.Assets.Domain.Assets;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;

namespace AssetSystem.Assets.Application.Abstractions;

public interface IAssetsUnitOfWork : IUnitOfWork;

/// <summary>Write side: loads and stores whole <see cref="Asset"/> aggregates.</summary>
public interface IAssetRepository
{
    void Add(Asset asset);
    Task<Asset?> GetAsync(Guid companyId, Guid assetId, CancellationToken ct);

    /// <summary>Assets (with all items) that contain at least one of the given items.</summary>
    Task<IReadOnlyList<Asset>> GetByItemIdsAsync(Guid companyId, IReadOnlyCollection<Guid> itemIds, CancellationToken ct);
}

/// <summary>Hands out the next asset number / item code. Backed by database counters.</summary>
public interface IAssetNumbering
{
    Task<string> NextAssetNumberAsync(AssetCategory category, CancellationToken ct);

    Task<string> NextItemCodeAsync(
        AssetCategory category, Guid companyId, string companyTag, string branchTag, string typeCode, CancellationToken ct);
}

public sealed record AssetSummary(
    Guid Id, string AssetNumber, AssetCategory Category, string Description, string Vendor,
    int ItemCount, int AvailableCount, DateOnly PurchaseDate, decimal Price);

public sealed record ItemRow(
    Guid Id, string ItemCode, string TypeCode, string TypeName, string Description, string PartNumber,
    string SerialNumber, string KeyDetail, string? ControlNumber, string? Remarks, string Status,
    Guid AssetId, string AssetNumber, string AssetDescription);

public sealed record StatusCount(string Status, int Count);

public sealed record IdentifierConflicts(IReadOnlySet<string> Serials, IReadOnlySet<string> Keys);

/// <summary>Read side: projections straight from the database, no aggregates involved.</summary>
public interface IAssetQueries
{
    Task<PagedResult<AssetSummary>> SearchAssetsAsync(
        Guid companyId, AssetCategory? category, string? search, int page, int pageSize, CancellationToken ct);

    Task<PagedResult<ItemRow>> SearchItemsAsync(
        Guid companyId, ItemStatus? status, string? search, int page, int pageSize, CancellationToken ct);

    Task<ItemRow?> GetItemAsync(Guid companyId, Guid itemId, CancellationToken ct);

    /// <summary>Resolves a scanned/typed code: an asset number yields all of its items, an item code yields that item.</summary>
    Task<IReadOnlyList<ItemRow>> LookupByCodeAsync(Guid companyId, string code, CancellationToken ct);

    Task<IReadOnlyList<ItemRow>> GetItemsAsync(Guid companyId, IReadOnlyCollection<Guid> itemIds, CancellationToken ct);

    Task<IReadOnlyList<StatusCount>> ItemStatusSummaryAsync(Guid companyId, CancellationToken ct);

    Task<int> CountAssetsAsync(Guid companyId, CancellationToken ct);

    Task<IReadOnlyList<string>> SuggestAsync(Guid companyId, SuggestionField field, string? term, int limit, CancellationToken ct);

    /// <summary>Which of the given serial numbers / keys are already used by another item of the company.</summary>
    Task<IdentifierConflicts> FindIdentifierConflictsAsync(
        Guid companyId, IReadOnlyCollection<string> serials, IReadOnlyCollection<string> keys,
        Guid? excludeItemId, CancellationToken ct);
}

public enum SuggestionField { Vendor, AssetDescription, ItemDescription, PartNumber }
