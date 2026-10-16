using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Assets.Application;

public static class AssetsApplicationModule
{
    public static IServiceCollection AddAssetsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssetsApplicationModule).Assembly, ServiceLifetime.Singleton, includeInternalTypes: true);
        return services;
    }
}
