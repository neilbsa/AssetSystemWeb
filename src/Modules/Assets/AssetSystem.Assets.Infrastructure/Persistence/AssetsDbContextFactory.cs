using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetSystem.Assets.Infrastructure.Persistence;

/// <summary>Used only by <c>dotnet ef</c> to create migrations; the connection string is never used to connect.</summary>
internal sealed class AssetsDbContextFactory : IDesignTimeDbContextFactory<AssetsDbContext>
{
    public AssetsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AssetsDbContext>()
            .UseNpgsql("Host=localhost;Database=design_time", npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", AssetsDbContext.Schema))
            .Options;
        return new AssetsDbContext(options);
    }
}
