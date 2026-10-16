using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetSystem.Organization.Infrastructure.Persistence;

/// <summary>Used only by <c>dotnet ef</c> to create migrations; the connection string is never used to connect.</summary>
internal sealed class OrganizationDbContextFactory : IDesignTimeDbContextFactory<OrganizationDbContext>
{
    public OrganizationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrganizationDbContext>()
            .UseNpgsql("Host=localhost;Database=design_time", npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", OrganizationDbContext.Schema))
            .Options;
        return new OrganizationDbContext(options);
    }
}
