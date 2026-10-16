using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AssetSystem.BuildingBlocks.Infrastructure.Persistence;

/// <summary>Startup work owned by a module (apply migrations, seed reference data).</summary>
public interface IModuleInitializer
{
    Task InitializeAsync(IServiceProvider services, CancellationToken ct);
}

/// <summary>Migrates a module's schema. Derive one per module.</summary>
public abstract class MigratingInitializer<TContext> : IModuleInitializer where TContext : DbContext
{
    public async Task InitializeAsync(IServiceProvider services, CancellationToken ct)
    {
        var db = services.GetRequiredService<TContext>();
        await db.Database.MigrateAsync(ct);
        await SeedAsync(db, services, ct);
    }

    protected virtual Task SeedAsync(TContext db, IServiceProvider services, CancellationToken ct) => Task.CompletedTask;
}

/// <summary>Runs every module's initializers (in registration order) before the host starts serving requests.</summary>
public sealed class ModuleInitializerService(IServiceProvider root) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        await using var scope = root.CreateAsyncScope();
        foreach (var initializer in scope.ServiceProvider.GetServices<IModuleInitializer>())
            await initializer.InitializeAsync(scope.ServiceProvider, ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
