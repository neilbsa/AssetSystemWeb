using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Infrastructure.Persistence;
using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Assets.Infrastructure;

public static class AssetsInfrastructureModule
{
    public static IServiceCollection AddAssetsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddModuleDbContext<AssetsDbContext>(connectionString, AssetsDbContext.Schema);

        services.AddScoped<IAssetsUnitOfWork, AssetsUnitOfWork>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IAssetQueries, AssetQueries>();
        services.AddScoped<IAssetNumbering, AssetNumbering>();

        services.AddScoped<IModuleInitializer, AssetsInitializer>();
        return services;
    }
}
