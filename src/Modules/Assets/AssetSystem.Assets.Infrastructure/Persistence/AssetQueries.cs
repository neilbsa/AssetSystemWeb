using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Assets.Infrastructure.Persistence;

public sealed class AssetQueries(AssetsDbContext db) : IAssetQueries
{
    public async Task<PagedResult<AssetSummary>> SearchAssetsAsync(
        Guid companyId, AssetCategory? category, string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Assets.AsNoTracking().Where(a => a.CompanyId == companyId);
        if (category is not null) query = query.Where(a => a.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(a =>
                EF.Functions.ILike(a.AssetNumber, pattern) ||
                EF.Functions.ILike(a.Description, pattern) ||
                EF.Functions.ILike(a.Vendor, pattern) ||
                a.Items.Any(i => EF.Functions.ILike(i.ItemCode, pattern) || EF.Functions.ILike(i.SerialNumber, pattern)));
        }

        var total = await query.CountAsync(ct);
        var rows = await query
            .OrderByDescending(a => a.AssetNumber)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(a => new
            {
                a.Id, a.AssetNumber, a.Category, a.Description, a.Vendor,
                ItemCount = a.Items.Count(),
                AvailableCount = a.Items.Count(i => i.Status == ItemStatus.Available),
                PurchaseDate = a.Purchase.Date,
                Price = a.Purchase.Price
            })
            .ToListAsync(ct);

        var items = rows.Select(r => new AssetSummary(
            r.Id, r.AssetNumber, r.Category, r.Description, r.Vendor, r.ItemCount, r.AvailableCount, r.PurchaseDate, r.Price)).ToList();
        return new PagedResult<AssetSummary>(items, total, page, pageSize);
    }

    public async Task<PagedResult<ItemRow>> SearchItemsAsync(
        Guid companyId, ItemStatus? status, string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = Rows(companyId);
        if (status is not null) query = query.Where(r => r.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(r =>
                EF.Functions.ILike(r.ItemCode, pattern) ||
                EF.Functions.ILike(r.Description, pattern) ||
                EF.Functions.ILike(r.SerialNumber, pattern) ||
                EF.Functions.ILike(r.AssetNumber, pattern));
        }

        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(r => r.ItemCode).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<ItemRow>(rows.Select(ToRow).ToList(), total, page, pageSize);
    }

    public async Task<ItemRow?> GetItemAsync(Guid companyId, Guid itemId, CancellationToken ct)
    {
        var row = await Rows(companyId).FirstOrDefaultAsync(r => r.Id == itemId, ct);
        return row is null ? null : ToRow(row);
    }

    public async Task<IReadOnlyList<ItemRow>> LookupByCodeAsync(Guid companyId, string code, CancellationToken ct)
    {
        var normalised = code.Trim().ToUpperInvariant();
        var rows = await Rows(companyId)
            .Where(r => r.AssetNumber == normalised || r.ItemCode == normalised)
            .OrderBy(r => r.ItemCode)
            .ToListAsync(ct);
        return rows.Select(ToRow).ToList();
    }

    public async Task<IReadOnlyList<ItemRow>> GetItemsAsync(Guid companyId, IReadOnlyCollection<Guid> itemIds, CancellationToken ct)
    {
        var rows = await Rows(companyId).Where(r => itemIds.Contains(r.Id)).ToListAsync(ct);
        return rows.Select(ToRow).ToList();
    }

    public async Task<IReadOnlyList<StatusCount>> ItemStatusSummaryAsync(Guid companyId, CancellationToken ct)
    {
        var groups = await db.Items.AsNoTracking()
            .Where(i => i.CompanyId == companyId)
            .GroupBy(i => i.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        return groups.Select(g => new StatusCount(g.Status.ToString(), g.Count)).ToList();
    }

    public Task<int> CountAssetsAsync(Guid companyId, CancellationToken ct) =>
        db.Assets.CountAsync(a => a.CompanyId == companyId, ct);

    public async Task<IReadOnlyList<string>> SuggestAsync(
        Guid companyId, SuggestionField field, string? term, int limit, CancellationToken ct)
    {
        var pattern = $"%{term?.Trim()}%";
        var values = field switch
        {
            SuggestionField.Vendor => db.Assets.Where(a => a.CompanyId == companyId).Select(a => a.Vendor),
            SuggestionField.AssetDescription => db.Assets.Where(a => a.CompanyId == companyId).Select(a => a.Description),
            SuggestionField.ItemDescription => db.Items.Where(i => i.CompanyId == companyId).Select(i => i.Description),
            SuggestionField.PartNumber => db.Items.Where(i => i.CompanyId == companyId).Select(i => i.PartNumber),
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        return await values.Where(v => EF.Functions.ILike(v, pattern)).Distinct().OrderBy(v => v).Take(limit).ToListAsync(ct);
    }

    public async Task<IdentifierConflicts> FindIdentifierConflictsAsync(
        Guid companyId, IReadOnlyCollection<string> serials, IReadOnlyCollection<string> keys,
        Guid? excludeItemId, CancellationToken ct)
    {
        var items = db.Items.AsNoTracking().Where(i => i.CompanyId == companyId && (excludeItemId == null || i.Id != excludeItemId));

        var usedSerials = serials.Count == 0
            ? []
            : await items.Where(i => serials.Contains(i.SerialNumber)).Select(i => i.SerialNumber).ToListAsync(ct);
        var usedKeys = keys.Count == 0
            ? []
            : await items.Where(i => keys.Contains(i.KeyDetail)).Select(i => i.KeyDetail).ToListAsync(ct);

        return new IdentifierConflicts(usedSerials.ToHashSet(), usedKeys.ToHashSet());
    }

    // ---- helpers ----------------------------------------------------------------------------

    /// <summary>Flat item + asset columns; keeps filtering and paging in SQL.</summary>
    private sealed class Row
    {
        public Guid Id { get; init; }
        public string ItemCode { get; init; } = default!;
        public string TypeCode { get; init; } = default!;
        public string TypeName { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string PartNumber { get; init; } = default!;
        public string SerialNumber { get; init; } = default!;
        public string KeyDetail { get; init; } = default!;
        public string? ControlNumber { get; init; }
        public string? Remarks { get; init; }
        public ItemStatus Status { get; init; }
        public Guid AssetId { get; init; }
        public string AssetNumber { get; init; } = default!;
        public string AssetDescription { get; init; } = default!;
    }

    private IQueryable<Row> Rows(Guid companyId) =>
        from i in db.Items.AsNoTracking()
        join a in db.Assets.AsNoTracking() on EF.Property<Guid>(i, "AssetId") equals a.Id
        where a.CompanyId == companyId
        select new Row
        {
            Id = i.Id, ItemCode = i.ItemCode, TypeCode = i.TypeCode, TypeName = i.TypeName, Description = i.Description,
            PartNumber = i.PartNumber, SerialNumber = i.SerialNumber, KeyDetail = i.KeyDetail,
            ControlNumber = i.ControlNumber, Remarks = i.Remarks, Status = i.Status,
            AssetId = a.Id, AssetNumber = a.AssetNumber, AssetDescription = a.Description
        };

    private static ItemRow ToRow(Row r) => new(
        r.Id, r.ItemCode, r.TypeCode, r.TypeName, r.Description, r.PartNumber, r.SerialNumber, r.KeyDetail,
        r.ControlNumber, r.Remarks, r.Status.ToString(), r.AssetId, r.AssetNumber, r.AssetDescription);
}
