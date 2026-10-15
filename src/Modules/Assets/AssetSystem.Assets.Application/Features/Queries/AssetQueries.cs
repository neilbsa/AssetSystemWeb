using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Consignments.Contracts;
using AssetSystem.Organization.Contracts;
using AssetSystem.SharedKernel.Results;
using Wolverine;

namespace AssetSystem.Assets.Application.Features.Queries;

// ---- Asset list ---------------------------------------------------------------------------

public sealed record ListAssetsQuery(Guid CompanyId, AssetCategory? Category, string? Search, int? Page, int? PageSize);

public static class ListAssetsHandler
{
    public static Task<PagedResult<AssetSummary>> Handle(ListAssetsQuery q, IAssetQueries queries, CancellationToken ct)
    {
        var (page, size) = Paging.Normalize(q.Page, q.PageSize);
        return queries.SearchAssetsAsync(q.CompanyId, q.Category, q.Search, page, size, ct);
    }
}

// ---- Asset details ------------------------------------------------------------------------

public sealed record GetAssetQuery(Guid CompanyId, Guid AssetId);

public sealed record AssetItemResponse(
    Guid Id, string ItemCode, string TypeCode, string TypeName, string Description, string PartNumber,
    string SerialNumber, string KeyDetail, string? ControlNumber, string? Remarks, string Status);

public sealed record AssetDocumentResponse(Guid Id, string DocumentType, string FileName, string ContentType, long SizeBytes);

public sealed record AssetDetailsResponse(
    Guid Id, string AssetNumber, string Category, Guid BranchId, string BranchName,
    string Description, string Vendor, string? PurchaseOrderNumber, string? ControlNumber, string? Remarks,
    decimal Price, DateOnly PurchaseDate, int LifeSpanMonths, int WarrantyMonths, DateOnly WarrantyEndsOn,
    decimal DepreciatedValue, string InvoiceNumber, DateOnly InvoiceDate, Guid SupplierId, string SupplierName,
    IReadOnlyList<AssetItemResponse> Items, IReadOnlyList<AssetDocumentResponse> Documents,
    DateTimeOffset CreatedAt, string CreatedBy);

public static class GetAssetHandler
{
    public static async Task<Result<AssetDetailsResponse>> Handle(
        GetAssetQuery q, IAssetRepository assets, IMessageBus bus, TimeProvider clock, CancellationToken ct)
    {
        var asset = await assets.GetAsync(q.CompanyId, q.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");

        var branch = await bus.InvokeAsync<BranchDto?>(new GetBranch(q.CompanyId, asset.BranchId), ct);
        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);

        return new AssetDetailsResponse(
            asset.Id, asset.AssetNumber, asset.Category.ToString(), asset.BranchId, branch?.Name ?? "",
            asset.Description, asset.Vendor, asset.PurchaseOrderNumber, asset.ControlNumber, asset.Remarks,
            asset.Purchase.Price, asset.Purchase.Date, asset.Purchase.LifeSpanMonths, asset.Purchase.WarrantyMonths,
            asset.Purchase.WarrantyEndsOn, asset.DepreciatedValue(today),
            asset.Invoice.Number, asset.Invoice.Date, asset.Invoice.SupplierId, asset.Invoice.SupplierName,
            asset.Items.OrderBy(i => i.ItemCode).Select(ToResponse).ToList(),
            asset.Documents.Select(d => new AssetDocumentResponse(d.Id, d.DocumentType, d.FileName, d.ContentType, d.SizeBytes)).ToList(),
            asset.CreatedAt, asset.CreatedBy);
    }

    internal static AssetItemResponse ToResponse(AssetItem i) => new(
        i.Id, i.ItemCode, i.TypeCode, i.TypeName, i.Description, i.PartNumber,
        i.SerialNumber, i.KeyDetail, i.ControlNumber, i.Remarks, i.Status.ToString());
}

// ---- Item list / lookup -------------------------------------------------------------------

public sealed record ListItemsQuery(Guid CompanyId, ItemStatus? Status, string? Search, int? Page, int? PageSize);

public static class ListItemsHandler
{
    public static Task<PagedResult<ItemRow>> Handle(ListItemsQuery q, IAssetQueries queries, CancellationToken ct)
    {
        var (page, size) = Paging.Normalize(q.Page, q.PageSize);
        return queries.SearchItemsAsync(q.CompanyId, q.Status, q.Search, page, size, ct);
    }
}

public sealed record GetItemQuery(Guid CompanyId, Guid ItemId);

public static class GetItemHandler
{
    public static async Task<Result<ItemRow>> Handle(GetItemQuery q, IAssetQueries queries, CancellationToken ct)
    {
        var item = await queries.GetItemAsync(q.CompanyId, q.ItemId, ct);
        return item is null ? Error.NotFound("item.not_found", "Item not found.") : item;
    }
}

/// <summary>
/// Used by the consignment form: type an asset number to get all of its items, or an item code to get one.
/// Only items that can actually be issued (status Available) are returned.
/// </summary>
public sealed record LookupAvailableItemsQuery(Guid CompanyId, string Code);

public static class LookupAvailableItemsHandler
{
    public static async Task<IReadOnlyList<ItemRow>> Handle(LookupAvailableItemsQuery q, IAssetQueries queries, CancellationToken ct) =>
        string.IsNullOrWhiteSpace(q.Code)
            ? []
            : (await queries.LookupByCodeAsync(q.CompanyId, q.Code.Trim(), ct))
                .Where(i => i.Status == nameof(ItemStatus.Available))
                .ToList();
}

// ---- Item history (asks the Consignments module) ---------------------------------------------

public sealed record GetItemHistoryQuery(Guid CompanyId, Guid ItemId);

public sealed record ItemHistoryResponse(ItemRow Item, IReadOnlyList<ItemConsignmentDto> Consignments);

public static class GetItemHistoryHandler
{
    public static async Task<Result<ItemHistoryResponse>> Handle(
        GetItemHistoryQuery q, IAssetQueries queries, IMessageBus bus, CancellationToken ct)
    {
        var item = await queries.GetItemAsync(q.CompanyId, q.ItemId, ct);
        if (item is null) return Error.NotFound("item.not_found", "Item not found.");

        var history = await bus.InvokeAsync<IReadOnlyList<ItemConsignmentDto>>(new GetItemConsignmentHistory(q.CompanyId, q.ItemId), ct);
        return new ItemHistoryResponse(item, history);
    }
}

// ---- Dashboard ----------------------------------------------------------------------------

public sealed record GetDashboardQuery(Guid CompanyId);

public sealed record DashboardResponse(int Assets, int Items, IReadOnlyList<StatusCount> ItemsByStatus);

public static class GetDashboardHandler
{
    public static async Task<DashboardResponse> Handle(GetDashboardQuery q, IAssetQueries queries, CancellationToken ct)
    {
        var byStatus = await queries.ItemStatusSummaryAsync(q.CompanyId, ct);
        // Every status is always present so charts don't have to special-case zero.
        var complete = Enum.GetNames<ItemStatus>()
            .Select(name => new StatusCount(name, byStatus.FirstOrDefault(s => s.Status == name)?.Count ?? 0))
            .ToList();

        return new DashboardResponse(await queries.CountAssetsAsync(q.CompanyId, ct), complete.Sum(s => s.Count), complete);
    }
}

// ---- Suggestions and duplicate check (form helpers) ---------------------------------------------

public sealed record SuggestQuery(Guid CompanyId, SuggestionField Field, string? Term);

public static class SuggestHandler
{
    public static Task<IReadOnlyList<string>> Handle(SuggestQuery q, IAssetQueries queries, CancellationToken ct) =>
        queries.SuggestAsync(q.CompanyId, q.Field, q.Term, 10, ct);
}

public sealed record CheckIdentifiersQuery(Guid CompanyId, string? SerialNumber, string? KeyDetail, Guid? ExcludeItemId);

public sealed record IdentifierAvailabilityResponse(bool SerialInUse, bool KeyInUse);

public static class CheckIdentifiersHandler
{
    public static async Task<IdentifierAvailabilityResponse> Handle(CheckIdentifiersQuery q, IAssetQueries queries, CancellationToken ct)
    {
        var serial = AssetItem.NormalizeIdentifier(q.SerialNumber);
        var key = AssetItem.NormalizeIdentifier(q.KeyDetail);
        var serials = serial == AssetItem.NotApplicable ? [] : new[] { serial };
        var keys = key == AssetItem.NotApplicable ? [] : new[] { key };

        var conflicts = await queries.FindIdentifierConflictsAsync(q.CompanyId, serials, keys, q.ExcludeItemId, ct);
        return new IdentifierAvailabilityResponse(conflicts.Serials.Count > 0, conflicts.Keys.Count > 0);
    }
}
