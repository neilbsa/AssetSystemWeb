using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;

namespace AssetSystem.BuildingBlocks.Infrastructure.Persistence;

public static class ModuleDbContextExtensions
{
    /// <summary>
    /// Registers a module's DbContext: PostgreSQL, one schema per module, audit stamping,
    /// and Wolverine's EF Core integration (transactional outbox).
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(
        this IServiceCollection services, string connectionString, string schema)
        where TContext : DbContext
    {
        services.AddDbContextWithWolverineIntegration<TContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations", schema));
            options.AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });
        return services;
    }
}
