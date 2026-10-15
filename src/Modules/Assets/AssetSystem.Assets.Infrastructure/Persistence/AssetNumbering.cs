using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.BuildingBlocks.Infrastructure.Persistence;

namespace AssetSystem.Assets.Infrastructure.Persistence;

public sealed class AssetNumbering(AssetsDbContext db) : IAssetNumbering
{
    private readonly SequenceGenerator<AssetsDbContext> _sequences = new(db, AssetsDbContext.Schema);

    public async Task<string> NextAssetNumberAsync(AssetCategory category, CancellationToken ct) =>
        AssetCodes.AssetNumber(category, await _sequences.NextAsync(AssetCodes.AssetSequenceKey(category), ct));

    public async Task<string> NextItemCodeAsync(
        AssetCategory category, Guid companyId, string companyTag, string branchTag, string typeCode, CancellationToken ct)
    {
        var key = AssetCodes.ItemSequenceKey(category, companyId, branchTag, typeCode);
        return AssetCodes.ItemCode(category, companyTag, branchTag, typeCode, await _sequences.NextAsync(key, ct));
    }
}
