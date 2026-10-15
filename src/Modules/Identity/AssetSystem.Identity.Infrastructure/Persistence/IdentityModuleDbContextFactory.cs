using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetSystem.Identity.Infrastructure.Persistence;

/// <summary>Used only by <c>dotnet ef</c> to create migrations; the connection string is never used to connect.</summary>
internal sealed class IdentityModuleDbContextFactory : IDesignTimeDbContextFactory<IdentityModuleDbContext>
{
    public IdentityModuleDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityModuleDbContext>()
            .UseNpgsql("Host=localhost;Database=design_time", npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", IdentityModuleDbContext.Schema))
            .Options;
        return new IdentityModuleDbContext(options);
    }
}
