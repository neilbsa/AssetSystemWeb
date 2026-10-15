using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

/// <summary>Used only by <c>dotnet ef</c> to create migrations; the connection string is never used to connect.</summary>
internal sealed class ConsignmentsDbContextFactory : IDesignTimeDbContextFactory<ConsignmentsDbContext>
{
    public ConsignmentsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ConsignmentsDbContext>()
            .UseNpgsql("Host=localhost;Database=design_time", npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", ConsignmentsDbContext.Schema))
            .Options;
        return new ConsignmentsDbContext(options);
    }
}
