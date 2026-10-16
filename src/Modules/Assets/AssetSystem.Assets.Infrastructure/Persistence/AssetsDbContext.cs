using AssetSystem.Assets.Domain.Assets;
using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Assets.Infrastructure.Persistence;

public sealed class AssetsDbContext(DbContextOptions<AssetsDbContext> options) : DbContext(options)
{
    public const string Schema = "assets";

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetItem> Items => Set<AssetItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetsDbContext).Assembly);
        modelBuilder.AddSequences();
        modelBuilder.IgnoreDomainEvents();
    }
}
