using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Assets.Infrastructure.Persistence;

public sealed class AssetRepository(AssetsDbContext db) : IAssetRepository
{
    public void Add(Asset asset) => db.Assets.Add(asset);

    public Task<Asset?> GetAsync(Guid companyId, Guid assetId, CancellationToken ct) =>
        db.Assets
            .Include(a => a.Items)
            .Include(a => a.Documents)
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.CompanyId == companyId && a.Id == assetId, ct);

    public async Task<IReadOnlyList<Asset>> GetByItemIdsAsync(Guid companyId, IReadOnlyCollection<Guid> itemIds, CancellationToken ct) =>
        await db.Assets
            .Include(a => a.Items)
            .Where(a => a.CompanyId == companyId && a.Items.Any(i => itemIds.Contains(i.Id)))
            .ToListAsync(ct);
}
