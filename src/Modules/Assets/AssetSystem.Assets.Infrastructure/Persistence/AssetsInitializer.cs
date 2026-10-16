using AssetSystem.BuildingBlocks.Infrastructure.Persistence;

namespace AssetSystem.Assets.Infrastructure.Persistence;

internal sealed class AssetsInitializer : MigratingInitializer<AssetsDbContext>;
