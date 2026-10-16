using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Organization.Application;

public static class OrganizationApplicationModule
{
    /// <summary>Handlers are discovered by Wolverine (see host); validators are registered here.</summary>
    public static IServiceCollection AddOrganizationApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(OrganizationApplicationModule).Assembly, ServiceLifetime.Singleton, includeInternalTypes: true);
        return services;
    }
}
