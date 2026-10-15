using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Identity.Application;

public static class IdentityApplicationModule
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(IdentityApplicationModule).Assembly, ServiceLifetime.Singleton, includeInternalTypes: true);
        return services;
    }
}
