using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Consignments.Infrastructure;

public static class ConsignmentsInfrastructureModule
{
    public static IServiceCollection AddConsignmentsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddModuleDbContext<ConsignmentsDbContext>(connectionString, ConsignmentsDbContext.Schema);

        services.AddScoped<IConsignmentsUnitOfWork, ConsignmentsUnitOfWork>();
        services.AddScoped<IConsignmentRepository, ConsignmentRepository>();
        services.AddScoped<IConsignmentQueries, ConsignmentQueries>();
        services.AddScoped<IConsignmentNumbering, ConsignmentNumbering>();

        services.AddScoped<IModuleInitializer, ConsignmentsInitializer>();
        return services;
    }
}
