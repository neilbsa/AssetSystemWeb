using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Consignments.Application;

public static class ConsignmentsApplicationModule
{
    public static IServiceCollection AddConsignmentsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ConsignmentsApplicationModule).Assembly, ServiceLifetime.Singleton, includeInternalTypes: true);
        return services;
    }
}
